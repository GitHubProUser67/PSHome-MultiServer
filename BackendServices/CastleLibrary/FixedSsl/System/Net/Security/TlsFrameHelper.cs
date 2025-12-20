// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#if NET6_0_OR_GREATER
using EndianTools;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Authentication;
using System.Text;

namespace System.Net.Security
{
    // SSL3/TLS protocol frames definitions.
    internal enum TlsContentType : byte
    {
        ChangeCipherSpec = 20,
        Alert = 21,
        Handshake = 22,
        AppData = 23
    }

    internal enum TlsHandshakeType : byte
    {
        HelloRequest = 0,
        ClientHello = 1,
        ServerHello = 2,
        NewSessionTicket = 4,
        EndOfEarlyData = 5,
        EncryptedExtensions = 8,
        Certificate = 11,
        ServerKeyExchange = 12,
        CertificateRequest = 13,
        ServerHelloDone = 14,
        CertificateVerify = 15,
        ClientKeyExchange = 16,
        Finished = 20,
        KeyUpdate = 24,
        MessageHash = 254
    }

    internal enum TlsAlertLevel : byte
    {
        Warning = 1,
        Fatal = 2,
    }

    internal enum TlsAlertDescription : byte
    {
        CloseNotify = 0, // warning
        UnexpectedMessage = 10, // error
        BadRecordMac = 20, // error
        DecryptionFailed = 21, // reserved
        RecordOverflow = 22, // error
        DecompressionFail = 30, // error
        HandshakeFailure = 40, // error
        BadCertificate = 42, // warning or error
        UnsupportedCert = 43, // warning or error
        CertificateRevoked = 44, // warning or error
        CertificateExpired = 45, // warning or error
        CertificateUnknown = 46, // warning or error
        IllegalParameter = 47, // error
        UnknownCA = 48, // error
        AccessDenied = 49, // error
        DecodeError = 50, // error
        DecryptError = 51, // error
        ExportRestriction = 60, // reserved
        ProtocolVersion = 70, // error
        InsuffientSecurity = 71, // error
        InternalError = 80, // error
        UserCanceled = 90, // warning or error
        NoRenegotiation = 100, // warning
        UnsupportedExt = 110, // error
    }

    internal enum ExtensionType : ushort
    {
        ServerName = 0,
        MaximumFagmentLength = 1,
        ClientCertificateUrl = 2,
        TrustedCaKeys = 3,
        TruncatedHmac = 4,
        CertificateStatusRequest = 5,
        ApplicationProtocols = 16,
        SupportedVersions = 43,
        KeyShare = 51,
    }

    internal struct TlsFrameHeader
    {
        public TlsContentType Type;
        public SslProtocols Version;
        public int Length;

        public override string ToString() => $"{Version}:{Type}[{Length}]";
    }

    internal static class TlsFrameHelper
    {
        public const int HeaderSize = 5;

        [Flags]
        public enum ProcessingOptions
        {
            All = 0,
            ServerName = 0x1,
            ApplicationProtocol = 0x2,
            Versions = 0x4,
            RawApplicationProtocol = 0x8,
        }

        [Flags]
        public enum ApplicationProtocolInfo
        {
            None = 0,
            Http11 = 1,
            Http2 = 2,
            Other = 128
        }

        public struct TlsFrameInfo
        {
            public TlsFrameHeader Header;
            public TlsHandshakeType HandshakeType;
            public SslProtocols SupportedProtocols;
            public List<int> SupportedVersions;
            public string TargetName;
            public ApplicationProtocolInfo ApplicationProtocols;
            public TlsAlertDescription AlertDescription;
            public byte[] RawApplicationProtocols;

            public override string ToString()
            {
                if (Header.Type == TlsContentType.Handshake)
                {
                    if (HandshakeType == TlsHandshakeType.ClientHello)
                    {
                        return $"{Header.Version}:{HandshakeType}[{Header.Length}] TargetName='{TargetName}' SupportedVersion='{SupportedProtocols}' ApplicationProtocols='{ApplicationProtocols}'";
                    }
                    else if (HandshakeType == TlsHandshakeType.ServerHello)
                    {
                        return $"{Header.Version}:{HandshakeType}[{Header.Length}] SupportedVersion='{SupportedProtocols}' ApplicationProtocols='{ApplicationProtocols}'";
                    }
                    else
                    {
                        return $"{Header.Version}:{HandshakeType}[{Header.Length}] SupportedVersion='{SupportedProtocols}'";
                    }
                }
                else
                {
                    return $"{Header.Version}:{Header.Type}[{Header.Length}]";
                }
            }
        }

        public delegate bool HelloExtensionCallback(ref TlsFrameInfo info, ExtensionType type, ReadOnlySpan<byte> extensionsData);

        private static readonly byte[] s_protocolMismatch13 = new byte[] { (byte)TlsContentType.Alert, 3, 4, 0, 2, 2, 70 };
        private static readonly byte[] s_protocolMismatch12 = new byte[] { (byte)TlsContentType.Alert, 3, 3, 0, 2, 2, 70 };
        private static readonly byte[] s_protocolMismatch11 = new byte[] { (byte)TlsContentType.Alert, 3, 2, 0, 2, 2, 70 };
        private static readonly byte[] s_protocolMismatch10 = new byte[] { (byte)TlsContentType.Alert, 3, 1, 0, 2, 2, 70 };
        private static readonly byte[] s_protocolMismatch30 = new byte[] { (byte)TlsContentType.Alert, 3, 0, 0, 2, 2, 40 };

        private const int UInt24Size = 3;
        private const int RandomSize = 32;
        private const int ProtocolVersionMajorOffset = 0;
        private const int ProtocolVersionMinorOffset = 1;
        private const int ProtocolVersionSize = 2;
        private const int ProtocolVersionTlsMajorValue = 3;

        public static bool TryGetFrameHeader(ReadOnlySpan<byte> frame, ref TlsFrameHeader header)
        {
            if (frame.Length < HeaderSize)
            {
                header.Length = -1;
                return false;
            }

            header.Type = (TlsContentType)frame[0];

            // SSLv3, TLS or later
            if (frame[1] == 3)
            {
                header.Length = ((frame[3] << 8) | frame[4]) + HeaderSize;
                header.Version = TlsMinorVersionToProtocol(frame[2]);
            }
            else if (frame[2] == (byte)TlsHandshakeType.ClientHello &&
                     frame[3] == 3) // SSL3 or above
            {
                int length;
                if ((frame[0] & 0x80) != 0)
                {
                    // Two bytes
                    length = (((frame[0] & 0x7f) << 8) | frame[1]) + 2;
                }
                else
                {
                    // Three bytes
                    length = (((frame[0] & 0x3f) << 8) | frame[1]) + 3;
                }

                // max frame for SSLv2 is 32767.
                // However, we expect something reasonable for initial HELLO
                // We don't have enough logic to verify full validity,
                // the limits bellow are queses.
#pragma warning disable CS0618 // Ssl2 and Ssl3 are obsolete
                header.Version = SslProtocols.Ssl2;
#pragma warning restore CS0618
                header.Length = length;
                header.Type = TlsContentType.Handshake;
            }
            else
                header.Length = -1;

            return true;
        }

        // This function will try to parse TLS hello frame and fill details in provided info structure.
        // If frame was fully processed without any error, function returns true.
        // Otherwise it returns false and info may have partial data.
        // It is OK to call it again if more data becomes available.
        // It is also possible to limit what information is processed.
        // If callback delegate is provided, it will be called on ALL extensions.
        public static bool TryGetFrameInfo(ReadOnlySpan<byte> frame, ref TlsFrameInfo info, ProcessingOptions options = ProcessingOptions.All, HelloExtensionCallback callback = null)
        {
            const int HandshakeTypeOffset = 5;
            if (frame.Length < HeaderSize)
                return false;

            // This will not fail since we have enough data.
            bool gotHeader = TryGetFrameHeader(frame, ref info.Header);
            Debug.Assert(gotHeader);

            info.SupportedProtocols = info.Header.Version;

            if (info.Header.Type == TlsContentType.Alert)
            {
                TlsAlertLevel level = default;
                TlsAlertDescription description = default;
                if (TryGetAlertInfo(frame, ref level, ref description))
                {
                    info.AlertDescription = description;
                    return true;
                }

                return false;
            }

            if (info.Header.Type != TlsContentType.Handshake || frame.Length <= HandshakeTypeOffset)
                return false;

            info.HandshakeType = (TlsHandshakeType)frame[HandshakeTypeOffset];
#pragma warning disable CS0618 // Ssl2 and Ssl3 are obsolete
            if (info.Header.Version == SslProtocols.Ssl2)
            {
                // This is safe. We would not get here if the length is too small.
                info.SupportedProtocols |= TlsMinorVersionToProtocol(frame[4]);
                // We only recognize Unified ClientHello at the moment.
                // This is needed to trigger certificate selection callback in SslStream.
                info.HandshakeType = TlsHandshakeType.ClientHello;
                // There is no more parsing for old protocols.
                return true;
            }
#pragma warning restore CS0618

            // Check if we have full frame.
            bool isComplete = frame.Length >= info.Header.Length;

#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
            if (((int)info.Header.Version >= (int)SslProtocols.Tls) &&
#pragma warning restore SYSLIB0039
                (info.HandshakeType == TlsHandshakeType.ClientHello || info.HandshakeType == TlsHandshakeType.ServerHello))
            {
                if (!TryParseHelloFrame(frame.Slice(HeaderSize), ref info, options, callback))
                    isComplete = false;
            }

            return isComplete;
        }

        // This is similar to TryGetFrameInfo but it will only process SNI.
        // It returns TargetName as string or NULL if SNI is missing or parsing error happened.
        public static string GetServerName(ReadOnlySpan<byte> frame)
        {
            TlsFrameInfo info = default;
            if (!TryGetFrameInfo(frame, ref info, ProcessingOptions.ServerName))
                return null;

            return info.TargetName;
        }

        // This function will parse TLS Alert message and it will return alert level and description.
        public static bool TryGetAlertInfo(ReadOnlySpan<byte> frame, ref TlsAlertLevel level, ref TlsAlertDescription description)
        {
            if (frame.Length < 7 || frame[0] != (byte)TlsContentType.Alert)
                return false;

            level = (TlsAlertLevel)frame[5];
            description = (TlsAlertDescription)frame[6];

            return true;
        }

        private static byte[] CreateProtocolVersionAlert(SslProtocols version) =>
            version switch
            {
                SslProtocols.Tls13 => s_protocolMismatch13,
                SslProtocols.Tls12 => s_protocolMismatch12,
#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
                SslProtocols.Tls11 => s_protocolMismatch11,
                SslProtocols.Tls => s_protocolMismatch10,
#pragma warning restore SYSLIB0039
#pragma warning disable 0618
                SslProtocols.Ssl3 => s_protocolMismatch30,
#pragma warning restore 0618
                _ => Array.Empty<byte>(),
            };

        public static byte[] CreateAlertFrame(SslProtocols version, TlsAlertDescription reason)
        {
            if (reason == TlsAlertDescription.ProtocolVersion)
                return CreateProtocolVersionAlert(version);
#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
            else if ((int)version > (int)SslProtocols.Tls)
#pragma warning restore SYSLIB0039
            {
                // Create TLS1.2 alert
                byte[] buffer = new byte[] { (byte)TlsContentType.Alert, 3, 3, 0, 2, 2, (byte)reason };
                switch (version)
                {
                    case SslProtocols.Tls13:
                        buffer[2] = 4;
                        break;
#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
                    case SslProtocols.Tls11:
                        buffer[2] = 2;
                        break;
                    case SslProtocols.Tls:
                        buffer[2] = 1;
                        break;
#pragma warning restore SYSLIB0039
                }

                return buffer;
            }

            return Array.Empty<byte>();
        }

        private static bool TryParseHelloFrame(ReadOnlySpan<byte> sslHandshake, ref TlsFrameInfo info, ProcessingOptions options, HelloExtensionCallback callback)
        {
            // https://tools.ietf.org/html/rfc6101#section-5.6
            // struct {
            //     HandshakeType msg_type;    /* handshake type */
            //     uint24 length;             /* bytes in message */
            //     select (HandshakeType) {
            //         ...
            //         case client_hello: ClientHello;
            //         case server_hello: ServerHello;
            //         ...
            //     } body;
            // } Handshake;
            const int HandshakeTypeOffset = 0;
            const int HelloLengthOffset = HandshakeTypeOffset + sizeof(TlsHandshakeType);
            const int HelloOffset = HelloLengthOffset + UInt24Size;

            if (sslHandshake.Length < HelloOffset ||
                ((TlsHandshakeType)sslHandshake[HandshakeTypeOffset] != TlsHandshakeType.ClientHello &&
                 (TlsHandshakeType)sslHandshake[HandshakeTypeOffset] != TlsHandshakeType.ServerHello))
                return false;

            int helloLength = EndianAwareConverter.ToUInt24(sslHandshake.Slice(HelloLengthOffset), Endianness.BigEndian, 0);
            ReadOnlySpan<byte> helloData = sslHandshake.Slice(HelloOffset);

            if (helloData.Length < helloLength)
                return false;

            // ProtocolVersion may be different from frame header.
            if (helloData[ProtocolVersionMajorOffset] == ProtocolVersionTlsMajorValue)
                info.SupportedProtocols |= TlsMinorVersionToProtocol(helloData[ProtocolVersionMinorOffset]);

            return (TlsHandshakeType)sslHandshake[HandshakeTypeOffset] == TlsHandshakeType.ClientHello ?
                        TryParseClientHello(helloData.Slice(0, helloLength), ref info, options, callback) :
                        TryParseServerHello(helloData.Slice(0, helloLength), ref info, options, callback);
        }

        private static bool TryParseClientHello(ReadOnlySpan<byte> clientHello, ref TlsFrameInfo info, ProcessingOptions options, HelloExtensionCallback callback)
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

            ReadOnlySpan<byte> p = SniHelper.SkipBytes(clientHello, ProtocolVersionSize + RandomSize);

            // Skip SessionID (max size 32 => size fits in 1 byte)
            p = SniHelper.SkipOpaqueType1(p);

            // Skip cipher suites (max size 2^16-1 => size fits in 2 bytes)
            p = SniHelper.SkipOpaqueType2(p, out _);

            // Skip compression methods (max size 2^8-1 => size fits in 1 byte)
            p = SniHelper.SkipOpaqueType1(p);

            // no extensions
            if (p.IsEmpty)
                return true;

            // client_hello_extension_list (max size 2^16-1 => size fits in 2 bytes)
            int extensionListLength = EndianAwareConverter.ToUInt16(p, Endianness.BigEndian, 0);
            p = SniHelper.SkipBytes(p, sizeof(ushort));
            if (extensionListLength != p.Length)
                return false;

            return TryParseHelloExtensions(p, ref info, options, callback);
        }

        private static bool TryParseServerHello(ReadOnlySpan<byte> serverHello, ref TlsFrameInfo info, ProcessingOptions options, HelloExtensionCallback callback)
        {
            // Basic structure: https://tools.ietf.org/html/rfc6101#section-5.6.1.3
            // Extended structure: https://tools.ietf.org/html/rfc3546#section-2.2
            // struct {
            //   ProtocolVersion server_version;
            //   Random random;
            //   SessionID session_id;
            //   CipherSuite cipher_suite;
            //   CompressionMethod compression_method;
            //   Extension server_hello_extension_list<0..2^16-1>;
            // }
            // ServerHello;
            const int CipherSuiteLength = 2;
            const int CompressionMethiodLength = 1;

            ReadOnlySpan<byte> p = SniHelper.SkipBytes(serverHello, ProtocolVersionSize + RandomSize);
            // Skip SessionID (max size 32 => size fits in 1 byte)
            p = SniHelper.SkipOpaqueType1(p);
            p = SniHelper.SkipBytes(p, CipherSuiteLength + CompressionMethiodLength);

            // is invalid structure or no extensions?
            if (p.IsEmpty)
                return false;

            // client_hello_extension_list (max size 2^16-1 => size fits in 2 bytes)
            int extensionListLength = EndianAwareConverter.ToUInt16(p, Endianness.BigEndian, 0);
            p = SniHelper.SkipBytes(p, sizeof(ushort));
            if (extensionListLength != p.Length)
                return false;

            return TryParseHelloExtensions(p, ref info, options, callback);
        }

        // This is common for ClientHello and ServerHello.
        private static bool TryParseHelloExtensions(ReadOnlySpan<byte> extensions, ref TlsFrameInfo info, ProcessingOptions options, HelloExtensionCallback callback)
        {
            const int ExtensionHeader = 4;
            bool isComplete = true;
            bool hasSniSet = false; // (RFC 6066 §3)
            int ushortSizeOf = sizeof(ushort);

            while (extensions.Length >= ExtensionHeader)
            {
                ExtensionType extensionType = (ExtensionType)EndianAwareConverter.ToUInt16(extensions, Endianness.BigEndian, 0);
                extensions = SniHelper.SkipBytes(extensions, ushortSizeOf);

                ushort extensionLength = EndianAwareConverter.ToUInt16(extensions, Endianness.BigEndian, 0);
                extensions = SniHelper.SkipBytes(extensions, ushortSizeOf);
                if (extensions.Length < extensionLength)
                {
                    // If we have SNI, we don't need any more data even if fragmented.
                    isComplete = hasSniSet;
                    break;
                }

                ReadOnlySpan<byte> extensionData = extensions.Slice(0, extensionLength);

                if (extensionType == ExtensionType.ServerName && (options == ProcessingOptions.All ||
                   (options & ProcessingOptions.ServerName) == ProcessingOptions.ServerName))
                {
                    if (!TryGetSniFromServerNameList(extensionData, out string sni))
                        return false;

                    if (hasSniSet)
                        return false; // Not RFC compliant (exploit?).
                    hasSniSet = true;
                    info.TargetName = sni;
                }
                else if (extensionType == ExtensionType.SupportedVersions && (options == ProcessingOptions.All ||
                          (options & ProcessingOptions.Versions) == ProcessingOptions.Versions))
                {
                    if (!TryGetSupportedVersionsFromExtension(extensionData, out SslProtocols versions, out info.SupportedVersions))
                        return false;

                    info.SupportedProtocols |= versions;
                }
                else if (extensionType == ExtensionType.ApplicationProtocols && (options == ProcessingOptions.All ||
                          options.HasFlag(ProcessingOptions.ApplicationProtocol) || options.HasFlag(ProcessingOptions.RawApplicationProtocol)))
                {
                    if (!TryGetApplicationProtocolsFromExtension(extensionData, out ApplicationProtocolInfo alpn))
                        return false;

                    info.ApplicationProtocols |= alpn;

                    // Process RAW options only if explicitly set since that will allocate....
                    if (options.HasFlag(ProcessingOptions.RawApplicationProtocol))
                        // Skip ALPN extension Length. We have that in span.
                        info.RawApplicationProtocols = extensionData.Slice(ushortSizeOf).ToArray();
                }
                else if (extensionType == ExtensionType.KeyShare) // We must parse it all the time to prevent invalid payloads messing up our parsing.
                {
                    bool endOfkeyShare = false;
                    int cursor = ushortSizeOf;

                    // Read total length of key share list
                    ushort clientKeyShareLength = EndianAwareConverter.ToUInt16(extensionData, Endianness.BigEndian, 0);
                    extensionData = extensionData.Slice(ushortSizeOf);

                    ReadOnlySpan<byte> keyShareData = extensionData.Slice(0, clientKeyShareLength);

                    while (keyShareData.Length >= 4) // minimum size: 2 bytes group + 2 bytes key length
                    {
                        // Read group
                        ushort group = EndianAwareConverter.ToUInt16(keyShareData, Endianness.BigEndian, 0);
                        keyShareData = keyShareData.Slice(ushortSizeOf);

                        // Read key length
                        ushort keyLength = EndianAwareConverter.ToUInt16(keyShareData, Endianness.BigEndian, 0);
                        keyShareData = keyShareData.Slice(ushortSizeOf);

                        if (keyLength > keyShareData.Length) // Caused by some invalid extensions data such as REW_ROUTER_CUST.
                        {
                            endOfkeyShare = true;
                            keyLength = (ushort)keyShareData.Length;
                            cursor += keyLength;
                        }

                        // Read key bytes
                        ReadOnlySpan<byte> keyBytes = keyShareData.Slice(0, keyLength);
                        keyShareData = keyShareData.Slice(keyLength);
#if DEBUG
                        CustomLogger.LoggerAccessor.LogInfo($"[TlsFrameHelper] - Group: {group}, KeyLength: {keyLength}, Cursor: {cursor}");
#endif
                        if (endOfkeyShare)
                            break;

                        cursor += keyLength + 4; // 2 bytes group + 2 bytes length + key bytes
                    }

                    callback?.Invoke(ref info, extensionType, extensionData);
                    extensions = extensions.Slice(cursor);

                    continue;
                }

                callback?.Invoke(ref info, extensionType, extensionData);
                extensions = extensions.Slice(extensionLength);
            }

            return isComplete;
        }

        private static bool TryGetSniFromServerNameList(ReadOnlySpan<byte> serverNameListExtension, out string sni)
        {
            // https://tools.ietf.org/html/rfc3546#section-3.1
            // struct {
            //     ServerName server_name_list<1..2^16-1>
            // } ServerNameList;
            // ServerNameList is an opaque type (length of sufficient size for max data length is prepended)
            const int ServerNameListOffset = sizeof(ushort);
            sni = null;

            if (serverNameListExtension.Length < ServerNameListOffset)
                return false;

            int serverNameListLength = EndianAwareConverter.ToUInt16(serverNameListExtension, Endianness.BigEndian, 0);
            ReadOnlySpan<byte> serverNameList = serverNameListExtension.Slice(ServerNameListOffset);

            if (serverNameListLength != serverNameList.Length)
                return false;

            ReadOnlySpan<byte> serverName = serverNameList.Slice(0, serverNameListLength);

            sni = GetSniFromServerName(serverName, out bool invalid);
            return !invalid;
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
            const int NameTypeOffset = 0;
            const int HostNameStructOffset = NameTypeOffset + sizeof(SniHelper.NameType);
            if (serverName.Length < HostNameStructOffset)
            {
                invalid = true;
                return null;
            }

            // Following can underflow but it is ok due to equality check below
            SniHelper.NameType nameType = (SniHelper.NameType)serverName[NameTypeOffset];
            ReadOnlySpan<byte> hostNameStruct = serverName.Slice(HostNameStructOffset);
            if (nameType != SniHelper.NameType.HostName)
            {
                invalid = true;
                return null;
            }

            return SniHelper.GetSniFromHostNameStruct(hostNameStruct, out invalid);
        }

        private static bool TryGetSupportedVersionsFromExtension(ReadOnlySpan<byte> extensionData, out SslProtocols protocols, out List<int> versions)
        {
            // https://tools.ietf.org/html/rfc8446#section-4.2.1
            // struct {
            // select(Handshake.msg_type) {
            //  case client_hello:
            //    ProtocolVersion versions<2..254 >;
            //
            //  case server_hello: /* and HelloRetryRequest */
            //    ProtocolVersion selected_version;
            // };
            const int VersionListLengthOffset = 0;
            const int VersionListNameOffset = VersionListLengthOffset + sizeof(byte);
            const int VersionLength = 2;

            protocols = SslProtocols.None;
            versions = new List<int>();

            byte supportedVersionLength = extensionData[VersionListLengthOffset];
            extensionData = extensionData.Slice(VersionListNameOffset);

            if (extensionData.Length != supportedVersionLength)
                return false;

            while (extensionData.Length >= VersionLength)
            {
                ushort version = EndianAwareConverter.ToUInt16(extensionData, Endianness.BigEndian, 0);

                if (extensionData[ProtocolVersionMajorOffset] == ProtocolVersionTlsMajorValue)
                    protocols |= TlsMinorVersionToProtocol(extensionData[ProtocolVersionMinorOffset]);

                extensionData = extensionData.Slice(VersionLength);

                versions.Add(version);
            }

            return true;
        }

        private static bool TryGetApplicationProtocolsFromExtension(ReadOnlySpan<byte> extensionData, out ApplicationProtocolInfo alpn)
        {
            // https://tools.ietf.org/html/rfc7301#section-3.1
            // opaque ProtocolName<1..2 ^ 8 - 1 >;
            //
            // struct {
            //   ProtocolName protocol_name_list<2..2^16-1>
            // }
            // ProtocolNameList;
            const int AlpnListLengthOffset = 0;
            const int AlpnListOffset = AlpnListLengthOffset + sizeof(short);

            alpn = ApplicationProtocolInfo.None;

            if (extensionData.Length < AlpnListOffset)
                return false;

            int AlpnListLength = EndianAwareConverter.ToUInt16(extensionData, Endianness.BigEndian, 0);
            ReadOnlySpan<byte> alpnList = extensionData.Slice(AlpnListOffset);
            if (AlpnListLength != alpnList.Length)
                return false;

            while (!alpnList.IsEmpty)
            {
                byte protocolLength = alpnList[0];
                if (alpnList.Length < protocolLength + 1)
                    return false;

                ReadOnlySpan<byte> protocol = alpnList.Slice(1, protocolLength);
                if (protocolLength == 2)
                {
                    if (protocol.SequenceEqual(SslApplicationProtocol.Http2.Protocol.Span))
                        alpn |= ApplicationProtocolInfo.Http2;
                    else
                        alpn |= ApplicationProtocolInfo.Other;
                }
                else if (protocolLength == SslApplicationProtocol.Http11.Protocol.Length &&
                         protocol.SequenceEqual(SslApplicationProtocol.Http11.Protocol.Span))
                    alpn |= ApplicationProtocolInfo.Http11;
                else
                    alpn |= ApplicationProtocolInfo.Other;

                alpnList = alpnList.Slice(protocolLength + 1);
            }

            return true;
        }

        private static SslProtocols TlsMinorVersionToProtocol(byte value)
        {
            return value switch
            {
                4 => SslProtocols.Tls13,
                3 => SslProtocols.Tls12,
#pragma warning disable SYSLIB0039 // TLS 1.0 and 1.1 are obsolete
                2 => SslProtocols.Tls11,
                1 => SslProtocols.Tls,
#pragma warning restore SYSLIB0039
#pragma warning disable 0618
                0 => SslProtocols.Ssl3,
#pragma warning restore 0618
                _ => SslProtocols.None,
            };
        }
    }
}
#endif