// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using EndianTools;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Net.Security
{
    internal class SniHelper
    {
        private const int ProtocolVersionSize = 2;
        private const int UInt24Size = 3;
        private const int RandomSize = 32;
        private readonly static IdnMapping s_idnMapping = CreateIdnMapping();
        private readonly static Encoding s_encoding = CreateEncoding();

        public static (int, string) GetServerName(byte[] clientHello, List<int> versions)
        {
            return GetFromSslPlainText(clientHello, versions);
        }

        private static (int, string) GetFromSslPlainText(ReadOnlySpan<byte> sslPlainText, List<int> versions)
        {
            // https://tools.ietf.org/html/rfc6101#section-5.2.1
            // struct {
            //     ContentType type; // enum with max value 255
            //     ProtocolVersion version; // 2x uint8
            //     uint16 length;
            //     opaque fragment[SSLPlaintext.length];
            // } SSLPlaintext;
            const int ContentTypeOffset = 0;
            const int ProtocolVersionOffset = ContentTypeOffset + sizeof(ContentType);
            const int LengthOffset = ProtocolVersionOffset + ProtocolVersionSize;
            const int HandshakeOffset = LengthOffset + sizeof(ushort);

            // Skip ContentType and ProtocolVersion
            ushort handshakeLength = EndianAwareConverter.ToUInt16(sslPlainText.Slice(LengthOffset), Endianness.BigEndian, 0);
            ReadOnlySpan<byte> sslHandshake = sslPlainText.Slice(HandshakeOffset);

            if (handshakeLength != sslHandshake.Length)
                return (-5, null);

            return GetFromSslHandshake(sslHandshake, versions);
        }

        private static (int, string) GetFromSslHandshake(ReadOnlySpan<byte> sslHandshake, List<int> versions)
        {
            // https://tools.ietf.org/html/rfc6101#section-5.6
            // struct {
            //     HandshakeType msg_type;    /* handshake type */
            //     uint24 length;             /* bytes in message */
            //     select (HandshakeType) {
            //         ...
            //         case client_hello: ClientHello;
            //         ...
            //     } body;
            // } Handshake;
            const int HandshakeTypeOffset = 0;
            const int ClientHelloLengthOffset = HandshakeTypeOffset + sizeof(HandshakeType);
            const int ClientHelloOffset = ClientHelloLengthOffset + UInt24Size;

            if (sslHandshake.Length < ClientHelloOffset || (HandshakeType)sslHandshake[HandshakeTypeOffset] != HandshakeType.ClientHello)
                return (-5, null);

            int clientHelloLength = EndianAwareConverter.ToUInt24(sslHandshake.Slice(ClientHelloLengthOffset), Endianness.BigEndian, 0);
            ReadOnlySpan<byte> clientHello = sslHandshake.Slice(ClientHelloOffset);

            if (clientHello.Length != clientHelloLength)
                return (-5, null);

            return GetSniFromClientHello(clientHello, versions);
        }

        private static (int, string) GetSniFromClientHello(ReadOnlySpan<byte> clientHello, List<int> versions)
        {
            // Basic structure: https://tools.ietf.org/html/rfc6101#section-5.6.1.2
            // Extended structure: https://tools.ietf.org/html/rfc3546#section-2.1
            // struct {
            //     ProtocolVersion client_version; // 2x uint8
            //     Random random; // 32 bytes
            //     SessionID session_id; // opaque type
            //     CipherSuite cipher_suites<2..2^16-1>; // opaque type
            //     CompressionMethod compression_methods<1..2^8-1>; // opaque type
            //     Extension client_hello_extension_list<0..2^16-1>;
            // } ClientHello;
            ReadOnlySpan<byte> p = SkipBytes(clientHello, ProtocolVersionSize + RandomSize);

            // Skip SessionID (max size 32 => size fits in 1 byte)
            p = SkipOpaqueType1(p);

            // Skip cipher suites (max size 2^16-1 => size fits in 2 bytes)
            p = SkipOpaqueType2(p, out _);

            // Skip compression methods (max size 2^8-1 => size fits in 1 byte)
            p = SkipOpaqueType1(p);

            // is invalid structure or no extensions?
            if (p.IsEmpty)
                return (-5, null);

            // client_hello_extension_list (max size 2^16-1 => size fits in 2 bytes)
            ushort extensionListLength = EndianAwareConverter.ToUInt16(p, Endianness.BigEndian, 0);
            p = SkipBytes(p, sizeof(ushort));

            if (extensionListLength != p.Length)
                return (-5, null);

            bool hasSniSet = false; // (RFC 6066 §3)
            string ret = null;
            while (!p.IsEmpty)
            {
                var extensionRes = GetFromExtension(p, out p, out bool invalid);

                if (invalid)
                    return (-5, null);

                else if (extensionRes.HasValue)
                {
                    var extensionParams = extensionRes.Value;
                    switch (extensionParams.Item1)
                    {
                        case ExtensionType.ServerName:
                            {
                                if (hasSniSet)
                                    return (-5, null); // Not RFC compliant (exploit?).
                                else
                                {
                                    hasSniSet = true;
                                    ret = extensionParams.Item2;
                                }
                            }
                            break;
                        case ExtensionType.SupportedVersions:
                            {
                                versions.AddRange(extensionParams.Item2.Split(',')
                                    .Select(x => int.Parse(x)));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return (ret == null ? -2 : 0, ret);
        }

        private static (ExtensionType, string)? GetFromExtension(ReadOnlySpan<byte> extension, out ReadOnlySpan<byte> remainingBytes, out bool invalid)
        {
            // https://tools.ietf.org/html/rfc3546#section-2.3
            // struct {
            //     ExtensionType extension_type;
            //     opaque extension_data<0..2^16-1>;
            // } Extension;
            const int ExtensionDataOffset = sizeof(ExtensionType);

            if (extension.Length < ExtensionDataOffset)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                invalid = true;
                return null;
            }

            ExtensionType extensionType = (ExtensionType)EndianAwareConverter.ToUInt16(extension, Endianness.BigEndian, 0);
            ReadOnlySpan<byte> extensionData = extension.Slice(ExtensionDataOffset);

            switch (extensionType)
            {
                case ExtensionType.ServerName:
                    return (extensionType, GetSniFromServerNameList(extensionData, out remainingBytes, out invalid));
                case ExtensionType.SupportedVersions:
                    return (extensionType, string.Join(",", GetVersionsList(extensionData, out remainingBytes, out invalid)));
                default:
                    break;
            }

            remainingBytes = SkipOpaqueType2(extensionData, out invalid);
            return null;
        }

        private static List<int> GetVersionsList(ReadOnlySpan<byte> versionListExtension, out ReadOnlySpan<byte> remainingBytes, out bool invalid)
        {
            const int ServerVersionListOffset = sizeof(ushort);
            List<int> output = new List<int>() { };

            if (versionListExtension.Length < ServerVersionListOffset)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                invalid = true;
                return output;
            }

            ushort serverVersionsListLength = EndianAwareConverter.ToUInt16(versionListExtension, Endianness.BigEndian, 0);
            ReadOnlySpan<byte> serverVersionsList = versionListExtension.Slice(ServerVersionListOffset);

            if (serverVersionsListLength > serverVersionsList.Length)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                invalid = true;
                return output;
            }

            remainingBytes = serverVersionsList.Slice(serverVersionsListLength);
            ReadOnlySpan<byte> serverVersions = serverVersionsList.Slice(0, serverVersionsListLength);

            for (int i = 0; i < serverVersions[0]; i += 2)
                output.Add((serverVersions[i + 1] << 8) | serverVersions[i + 2]);

            invalid = false;
            return output;
        }

        private static string GetSniFromServerNameList(ReadOnlySpan<byte> serverNameListExtension, out ReadOnlySpan<byte> remainingBytes, out bool invalid)
        {
            // https://tools.ietf.org/html/rfc3546#section-3.1
            // struct {
            //     ServerName server_name_list<1..2^16-1>
            // } ServerNameList;
            // ServerNameList is an opaque type (length of sufficient size for max data length is prepended)
            const int ServerNameListOffset = sizeof(ushort);

            if (serverNameListExtension.Length < ServerNameListOffset)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                invalid = true;
                return null;
            }

            ushort serverNameListLength = EndianAwareConverter.ToUInt16(serverNameListExtension, Endianness.BigEndian, 0);
            ReadOnlySpan<byte> serverNameList = serverNameListExtension.Slice(ServerNameListOffset);

            if (serverNameListLength > serverNameList.Length)
            {
                remainingBytes = ReadOnlySpan<byte>.Empty;
                invalid = true;
                return null;
            }

            remainingBytes = serverNameList.Slice(serverNameListLength);
            ReadOnlySpan<byte> serverName = serverNameList.Slice(0, serverNameListLength);

            return GetSniFromServerName(serverName, out invalid);
        }

        private static string GetSniFromServerName(ReadOnlySpan<byte> serverName, out bool invalid)
        {
            // https://tools.ietf.org/html/rfc3546#section-3.1
            // struct {
            //     NameType name_type;
            //     select (name_type) {
            //         case host_name: HostName;
            //     } name;
            // } ServerName;
            // ServerName is an opaque type (length of sufficient size for max data length is prepended)
            const int ServerNameLengthOffset = 0;
            const int NameTypeOffset = ServerNameLengthOffset + sizeof(ushort);
            const int HostNameStructOffset = NameTypeOffset + sizeof(NameType);

            if (serverName.Length < HostNameStructOffset)
            {
                invalid = true;
                return null;
            }

            // Following can underflow but it is ok due to equality check below
            int hostNameStructLength = EndianAwareConverter.ToUInt16(serverName, Endianness.BigEndian, 0) - sizeof(NameType);
            NameType nameType = (NameType)serverName[NameTypeOffset];
            ReadOnlySpan<byte> hostNameStruct = serverName.Slice(HostNameStructOffset);

            if (hostNameStructLength != hostNameStruct.Length || nameType != NameType.HostName)
            {
                invalid = true;
                return null;
            }

            return GetSniFromHostNameStruct(hostNameStruct, out invalid);
        }

        private static string GetSniFromHostNameStruct(ReadOnlySpan<byte> hostNameStruct, out bool invalid)
        {
            // https://tools.ietf.org/html/rfc3546#section-3.1
            // HostName is an opaque type (length of sufficient size for max data length is prepended)
            const int HostNameLengthOffset = 0;
            const int HostNameOffset = HostNameLengthOffset + sizeof(ushort);

            ushort hostNameLength = EndianAwareConverter.ToUInt16(hostNameStruct, Endianness.BigEndian, 0);
            ReadOnlySpan<byte> hostName = hostNameStruct.Slice(HostNameOffset);
            if (hostNameLength != hostName.Length)
            {
                invalid = true;
                return null;
            }

            invalid = false;
            return DecodeString(hostName);
        }

        private static string DecodeString(ReadOnlySpan<byte> bytes)
        {
            // https://tools.ietf.org/html/rfc3546#section-3.1
            // Per spec:
            //   If the hostname labels contain only US-ASCII characters, then the
            //   client MUST ensure that labels are separated only by the byte 0x2E,
            //   representing the dot character U+002E (requirement 1 in section 3.1
            //   of [IDNA] notwithstanding). If the server needs to match the HostName
            //   against names that contain non-US-ASCII characters, it MUST perform
            //   the conversion operation described in section 4 of [IDNA], treating
            //   the HostName as a "query string" (i.e. the AllowUnassigned flag MUST
            //   be set). Note that IDNA allows labels to be separated by any of the
            //   Unicode characters U+002E, U+3002, U+FF0E, and U+FF61, therefore
            //   servers MUST accept any of these characters as a label separator.  If
            //   the server only needs to match the HostName against names containing
            //   exclusively ASCII characters, it MUST compare ASCII names case-
            //   insensitively.

            string idnEncodedString;
            try
            {
                idnEncodedString = s_encoding.GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            try
            {
                return s_idnMapping.GetUnicode(idnEncodedString);
            }
            catch (ArgumentException)
            {
                // client has not done IDN mapping
                return idnEncodedString;
            }
        }

        private static ReadOnlySpan<byte> SkipBytes(ReadOnlySpan<byte> bytes, int numberOfBytesToSkip)
        {
            return (numberOfBytesToSkip < bytes.Length) ? bytes.Slice(numberOfBytesToSkip) : ReadOnlySpan<byte>.Empty;
        }

        // Opaque type is of structure:
        //   - length (minimum number of bytes to hold the max value)
        //   - data (length bytes)
        // We will only use opaque types which are of max size: 255 (length = 1) or 2^16-1 (length = 2).
        // We will call them SkipOpaqueType`length`
        private static ReadOnlySpan<byte> SkipOpaqueType1(ReadOnlySpan<byte> bytes)
        {
            const int OpaqueTypeLengthSize = sizeof(byte);
            if (bytes.Length < OpaqueTypeLengthSize)
                return ReadOnlySpan<byte>.Empty;
            return SkipBytes(bytes, OpaqueTypeLengthSize + bytes[0]);
        }

        private static ReadOnlySpan<byte> SkipOpaqueType2(ReadOnlySpan<byte> bytes, out bool invalid)
        {
            const int OpaqueTypeLengthSize = sizeof(ushort);
            if (bytes.Length < OpaqueTypeLengthSize)
            {
                invalid = true;
                return ReadOnlySpan<byte>.Empty;
            }

            int totalBytes = OpaqueTypeLengthSize + EndianAwareConverter.ToUInt16(bytes, Endianness.BigEndian, 0);

            invalid = bytes.Length < totalBytes;
            if (invalid)
                return ReadOnlySpan<byte>.Empty;

            return bytes.Slice(totalBytes);
        }

        private static IdnMapping CreateIdnMapping()
        {
            return new IdnMapping()
            {
                // Per spec "AllowUnassigned flag MUST be set". See comment above GetSniFromServerNameList for more details.
                AllowUnassigned = true
            };
        }

        private static Encoding CreateEncoding()
        {
            return Encoding.GetEncoding("utf-8", new EncoderExceptionFallback(), new DecoderExceptionFallback());
        }

        private enum ContentType : byte
        {
            Handshake = 0x16
        }

        private enum HandshakeType : byte
        {
            ClientHello = 0x01
        }

        private enum ExtensionType : ushort
        {
            ServerName = 0x00,
            SupportedVersions = 0x002B
        }

        private enum NameType : byte
        {
            HostName = 0x00
        }
    }
}
#endif