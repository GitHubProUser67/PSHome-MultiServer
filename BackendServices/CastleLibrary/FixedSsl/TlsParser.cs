using System.Collections.Generic;
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
        private const int SERVER_NAME_LEN = 256;
        private const int TLS_HEADER_LEN = 5;
        private const byte TLS_HANDSHAKE_CONTENT_TYPE = 0x16;
        private const byte TLS_HANDSHAKE_TYPE_CLIENT_HELLO = 0x01;

        /// <summary>
        /// Parse a TLS packet for the Server Name Indication (SNI) extension.
        /// </summary>
        /// <param name="data">TLS record bytes</param>
        /// <param name="hostname">Extracted hostname, if found</param>
        /// <returns>
        ///  >=0  - length of hostname  
        ///  -1   - Incomplete request  
        ///  -2   - No Host header (SNI missing)  
        ///  -3   - Invalid hostname pointer  
        ///  -4   - Memory allocation failure (not applicable in managed code, but kept for parity)  
        ///  < -4 - Invalid TLS client hello
        /// </returns>
        public static int ParseTlsHeader(byte[] data, out string hostname, out List<int> versions)
        {
            hostname = null;
            versions = new List<int>();

            if (data == null)
                return -3;

            if (data.Length < TLS_HEADER_LEN)
                return -1;

            // SSL 2.0 Client Hello
            if ((data[0] & 0x80) != 0 && data[2] == 1)
                return -2;

            byte tlsContentType = data[0];
            if (tlsContentType != TLS_HANDSHAKE_CONTENT_TYPE)
                return -5;

            byte tlsVersionMajor = data[1];
            byte tlsVersionMinor = data[2];

            if (tlsVersionMajor < 3)
                return -2;

            int recordLen = (data[3] << 8) | data[4];
            recordLen += TLS_HEADER_LEN;

            if (data.Length < recordLen)
                return -1;

            int pos = TLS_HEADER_LEN;
            if (pos + 1 > data.Length)
                return -5;

            if (data[pos] != TLS_HANDSHAKE_TYPE_CLIENT_HELLO)
                return -5;

            // Skip: 1 (Handshake Type), 3 (Length), 2 (Version), 32 (Random)
            pos += 38;

            // Session ID
            if (pos + 1 > data.Length)
                return -5;
            int len = data[pos];
            pos += 1 + len;

            // Cipher Suites
            if (pos + 2 > data.Length)
                return -5;
            len = (data[pos] << 8) | data[pos + 1];
            pos += 2 + len;

            // Compression Methods
            if (pos + 1 > data.Length)
                return -5;
            len = data[pos];
            pos += 1 + len;

            if (pos == data.Length && tlsVersionMajor == 3 && tlsVersionMinor == 0)
                return -2; // SSL 3.0 without extensions

            // Extensions
            if (pos + 2 > data.Length)
                return -5;
            len = (data[pos] << 8) | data[pos + 1];
            pos += 2;

            if (pos + len > data.Length) 
                return -5;

            return ParseExtensions(data, pos, len, ref versions, out hostname);
        }

        private static int ParseExtensions(byte[] data, int offset, int dataLen, ref List<int> versions, out string hostname)
        {
            hostname = null;
            int pos = 0;

            while (pos + 4 <= dataLen)
            {
                int extType = (data[offset + pos] << 8) | data[offset + pos + 1];
                int len = (data[offset + pos + 2] << 8) | data[offset + pos + 3];

                if (extType == 0x0000) // Server Name extension
                {
                    if (pos + 4 + len > dataLen)
                        return -5;

                    return ParseServerNameExtension(data, offset + pos + 4, len, out hostname);
                }
                else if (extType == 0x002B) // Supported Versions extension
                {
                    int listLen = data[offset + pos + 4];
                    for (int i = 0; i < listLen; i += 2)
                    {
                        int ver = (data[offset + pos + 5 + i] << 8) | data[offset + pos + 5 + i + 1];
                        versions.Add(ver);
                    }
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
                int len = (data[offset + pos + 1] << 8) | data[offset + pos + 2];

                if (pos + 3 + len > dataLen)
                    return -5;

                if (nameType == 0x00) // host_name
                {
                    hostname = Encoding.ASCII.GetString(data, offset + pos + 3, len);
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
