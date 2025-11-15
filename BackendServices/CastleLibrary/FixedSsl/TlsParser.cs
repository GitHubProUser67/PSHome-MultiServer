using EndianTools;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

namespace FixedSsl
{
    // Modified from https://github.com/dlundquist/sniproxy/blob/master/src/tls.c with additional field checks.
    /*
     * Copyright (c) 2011 and 2012, Dustin Lundquist <dustin@null-ptr.net>
     * All rights reserved.
     *
     * Redistribution and use in source and binary forms, with or without
     * modification, are permitted provided that the following conditions are met:
     *
     * 1. Redistributions of source code must retain the above copyright notice,
     *    this list of conditions and the following disclaimer.
     * 2. Redistributions in binary form must reproduce the above copyright
     *    notice, this list of conditions and the following disclaimer in the
     *    documentation and/or other materials provided with the distribution.
     *
     * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
     * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
     * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
     * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
     * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
     * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
     * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
     * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
     * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
     * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
     * POSSIBILITY OF SUCH DAMAGE.
     */
    /*
     * This is a minimal TLS implementation intended only to parse the server name
     * extension.  This was created based primarily on Wireshark dissection of a
     * TLS handshake and RFC4366.
     */

    public static class TlsParser
    {
        private const int TLS_HEADER_LEN = 5;
        private const byte TLS_HANDSHAKE_CONTENT_TYPE = 0x16;
        private const byte TLS_HANDSHAKE_TYPE_CLIENT_HELLO = 0x01;

        private const int SSLV2_CLIENT_HELLO = 0x01;

        /// <summary>
        /// Parse a TLS packet for the Server Name Indication (SNI) extension.
        /// </summary>
        /// <param name="clientHello">TLS record bytes</param>
        /// <param name="hostname">Extracted hostname, if found</param>
        /// <returns>
        ///  >=0  - length of hostname  
        ///  -1   - Incomplete request  
        ///  -2   - No Host header (SNI missing)  
        ///  -3   - Invalid hostname pointer  
        ///  -4   - Memory allocation failure (not applicable in managed code, but kept for parity)  
        ///  < -4 - Invalid TLS client hello
        /// </returns>
        public static int ParseTlsHeader(byte[] clientHello, out string hostname, out bool isSslV2, out int maxSslVersion, out List<int> versions, out List<int> cipherSuites)
        {
            isSslV2 = false;
            maxSslVersion = -1;
            hostname = null;
            versions = new List<int>();
            cipherSuites = new List<int>();

            if (clientHello == null)
                return -3;

            if (clientHello.Length < TLS_HEADER_LEN)
                return -1;

            // SSL 2.0 Client Hello
            if (IsSslV2ClientHello(clientHello, out maxSslVersion, out versions, out cipherSuites))
            {
                isSslV2 = true;
                // SSLv2 doesn't support SNI, so return -2 (no hostname)
                return -2;
            }

            maxSslVersion = EndianAwareConverter.ToUInt16(clientHello, Endianness.BigEndian, 9);

            byte tlsContentType = clientHello[0];
            if (tlsContentType != TLS_HANDSHAKE_CONTENT_TYPE)
                return -5;

            byte tlsVersionMajor = clientHello[1];
            byte tlsVersionMinor = clientHello[2];

            if (tlsVersionMajor < 3)
                return -2;

            int recordLen = EndianAwareConverter.ToUInt16(clientHello, Endianness.BigEndian, 3);
            recordLen += TLS_HEADER_LEN;

            if (clientHello.Length < recordLen)
                return -1;

            int pos = TLS_HEADER_LEN;
            if (pos + 1 > clientHello.Length)
                return -5;

            if (clientHello[pos] != TLS_HANDSHAKE_TYPE_CLIENT_HELLO)
                return -5;

            // Skip: 1 (Handshake Type), 3 (Length), 2 (Version), 32 (Random)
            pos += 38;

            // Session ID
            if (pos + 1 > clientHello.Length)
                return -5;
            int len = clientHello[pos];
            pos += 1 + len;

            // Cipher Suites
            cipherSuites.AddRange(ParseCipherSuites(clientHello, ref pos));

            // Compression Methods
            if (pos + 1 > clientHello.Length)
                return -5;
            len = clientHello[pos];
            pos += 1 + len;

            if (pos == clientHello.Length && tlsVersionMajor == 3 && tlsVersionMinor == 0)
                return -2; // SSL 3.0 without extensions

            // Extensions
            if (pos + 2 > clientHello.Length)
                return -5;
            len = EndianAwareConverter.ToUInt16(clientHello, Endianness.BigEndian, (uint)pos);
            pos += 2;

            if (pos + len > clientHello.Length) 
                return -5;

            // Prefer using a modified version of the NETCORE 2.1 SNI parser
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            try
            {
                var sniHelperRes = SniHelper.GetServerName(clientHello, versions);

                if (sniHelperRes.Item1 != 0)
                    return sniHelperRes.Item1;

                hostname = sniHelperRes.Item2;
                return hostname.Length;
            }
            catch (Exception ex)
            {
#if DEBUG
                CustomLogger.LoggerAccessor.LogWarn($"[TlsParser] - DotNet SniHelper failed to parse attributes, falling back to managed implementation. (Exception:{ex})");
#endif
            }

            return ParseExtensions(clientHello, pos, len, ref versions, out hostname);
#else
            return ParseExtensions(clientHello, pos, len, ref versions, out hostname);
#endif
        }

        private static bool IsSslV2ClientHello(byte[] data, out int maxSslVersion, out List<int> versions, out List<int> cipherSuites)
        {
            maxSslVersion = -1;
            versions = new List<int>();
            cipherSuites = new List<int>();

            // SSLv2 Client Hello format:
            // Bytes 0-1: Length (high bit set, 15-bit length)
            // Byte 2: Message type (0x01 for Client Hello)
            // Bytes 3-4: Version (major, minor)
            // Byte 5-6: Cipher spec length
            // Byte 7-8: Session ID length
            // Byte 9-10: Challenge length
            // Followed by cipher specs, session ID, and challenge data

            if (data.Length < 11)
                return false;

            // Check for SSLv2 length indicator (high bit set in first byte)
            bool hasLengthIndicator = (data[0] & 0x80) != 0;
            bool isClientHelloType = data[2] == SSLV2_CLIENT_HELLO;

            if (!hasLengthIndicator || !isClientHelloType)
                return false;

            // Mark the version requested in the hello (can be more than SSLv2 such as in PS2 DNAS)
            maxSslVersion = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, 3);

            // Parse cipher specs to determine additional capabilities
            int cipherSpecLength = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, 5);
            int sessionIdLength = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, 7);
            int challengeLength = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, 9);

            int pos = 11;
            if (data.Length < pos + cipherSpecLength)
                return false;

            // SSLv2 cipher specs are of type Uint24
            for (int i = 0; i + 2 < cipherSpecLength; i += 3)
                cipherSuites.Add(EndianAwareConverter.ToUInt24(data, Endianness.BigEndian, (uint)(pos + i)));

            versions.Add(maxSslVersion);
            return true;
        }

        private static int[] ParseCipherSuites(byte[] data, ref int pos)
        {
            if (pos + 2 > data.Length)
                return Array.Empty<int>();

            int len = (data[pos] << 8) | data[pos + 1];
            pos += 2;

            // Must be even (each cipher is 2 bytes) and within bounds
            if (len % 2 != 0 || pos + len > data.Length)
                return Array.Empty<int>();

            int count = len / 2;
            int[] ciphers = new int[count];

            for (int i = 0; i < count; i++)
                ciphers[i] = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, (uint)(pos + i * 2));

            pos += len;
            return ciphers;
        }

        private static int ParseExtensions(byte[] data, int offset, int dataLen, ref List<int> versions, out string hostname)
        {
            hostname = null;
            int pos = 0;

            while (pos + 4 <= dataLen)
            {
                int extType = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, (uint)(offset + pos));
                int len = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, (uint)(offset + pos + 2));

                if (extType == 0x0000) // Server Name extension
                {
                    if (pos + 4 + len > dataLen)
                        return -5;

                    return ParseServerNameExtension(data, offset + pos + 4, len, out hostname);
                }
                else if (extType == 0x002B) // Supported Versions extension
                {
                    for (int i = 0; i < data[offset + pos + 4]; i += 2)
                        versions.Add(EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, (uint)(offset + pos + 5 + i)));
                    break;
                }

                pos += 4 + len;
            }

            if (pos != dataLen)
                return -5;

            return -2; // No SNI
        }

        private static int ParseServerNameExtension(byte[] data, int offset, int dataLen, out string hostname)
        {
            hostname = null;
            int pos = 2; // skip server name list length

            while (pos + 3 < dataLen)
            {
                int nameType = data[offset + pos];
                int len = EndianAwareConverter.ToUInt16(data, Endianness.BigEndian, (uint)(offset + pos + 1));

                if (pos + 3 + len > dataLen)
                    return -5;

                if (nameType == 0x00) // host_name
                {
                    byte[] hostnameBytes = new byte[len];
                    Array.Copy(data, offset + pos + 3, hostnameBytes, 0, len);
                    hostname = SniHelper.DecodeString(hostnameBytes);
                    return hostname.Length;
                }

                pos += 3 + len;
            }

            if (pos != dataLen)
                return -5;

            return -2;
        }
    }
}
