/*
 *   Mentalis.org Security Library
 *
 *     Copyright � 2002-2005, The Mentalis.org Team
 *     All rights reserved.
 *     http://www.mentalis.org/
 *
 *
 *   Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions
 *   are met:
 *
 *     - Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer.
 *
 *     - Neither the name of the Mentalis.org Team, nor the names of its contributors
 *        may be used to endorse or promote products derived from this
 *        software without specific prior written permission.
 *
 *   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 *   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 *   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 *   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 *   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 *   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 *   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 *   OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using CastleLibrary.FixedSsl.Security.Ssl.Ssl3;
using CastleLibrary.FixedSsl.Security.Ssl.Tls1;

namespace CastleLibrary.FixedSsl.Security.Ssl.Shared
{
    internal sealed class CompatibilityLayer
    {
        public CompatibilityLayer(SocketController controller, SecurityOptions options)
        {
            m_Buffer = Array.Empty<byte>();
            m_MinVersion = GetMinProtocol(options.Protocol);
            m_MaxVersion = GetMaxProtocol(options.Protocol);
            m_MinLayer =
                m_MinVersion.GetVersionInt() == 30
                    ? options.Entity == ConnectionEnd.Client
                        ? new RecordLayer(controller, new Ssl3ClientHandshakeLayer(null, options))
                        : new RecordLayer(controller, new Ssl3ServerHandshakeLayer(null, options))
                    : options.Entity == ConnectionEnd.Client
                        ? new RecordLayer(controller, new Tls1ClientHandshakeLayer(null, options))
                        : new RecordLayer(controller, new Tls1ServerHandshakeLayer(null, options));
            m_MinLayer.HandshakeLayer.RecordLayer = m_MinLayer;
            m_Options = options;
        }

        public byte[] GetClientHello()
        {
            m_Hello ??= m_MinLayer.GetControlBytes(ControlType.ClientHello);
            return m_Hello;
        }

        // return null if more bytes are needed
        // throws an SslException if the bytes are invalid
        // returns a RecordLayer instance if the method completed successfully
        public CompatibilityResult ProcessHello(byte[] bytes, int offset, int size)
        {
            return m_Options.Entity == ConnectionEnd.Client
                ? ProcessServerHello(bytes, offset, size)
                : ProcessClientHello(bytes, offset, size);
        }

        private CompatibilityResult ProcessServerHello(byte[] bytes, int offset, int size)
        {
            var temp = new byte[m_Buffer.Length + size];
            Array.Copy(m_Buffer, 0, temp, 0, m_Buffer.Length);
            Array.Copy(bytes, offset, temp, m_Buffer.Length, size);
            if (IsInvalidSsl3Hello(temp))
                throw new SslException(
                    AlertDescription.HandshakeFailure,
                    "The server hello message uses a protocol that was not recognized."
                );
            if (m_Buffer.Length + size < 11)
            { // not enough bytes
                m_Buffer = temp;
                return new CompatibilityResult(
                    null,
                    new SslRecordStatus(SslStatus.MessageIncomplete, null, null)
                );
            }
            var pv = new ProtocolVersion(temp[9], temp[10]);
            if (SupportsProtocol(m_Options.Protocol, pv))
            {
                if (m_MinLayer.HandshakeLayer.GetVersion().GetVersionInt() != pv.GetVersionInt())
                {
                    m_MinLayer.HandshakeLayer =
                        pv.GetVersionInt() == 30
                            ? new Ssl3ClientHandshakeLayer(m_MinLayer.HandshakeLayer)
                            : new Tls1ClientHandshakeLayer(m_MinLayer.HandshakeLayer);
                }
                return new CompatibilityResult(
                    m_MinLayer,
                    m_MinLayer.ProcessBytes(temp, 0, temp.Length)
                );
            }
            else
            {
                throw new SslException(
                    AlertDescription.HandshakeFailure,
                    "The client and server could not agree on the protocol version to use."
                );
            }
        }

        private static bool IsInvalidSsl3Hello(byte[] buffer)
        { // also works for TLS1 hellos
            return (buffer.Length > 0 && buffer[0] != 22)
                || (buffer.Length > 1 && buffer[1] != 3)
                || (buffer.Length > 2 && buffer[2] != 0 && buffer[2] != 1);
        }

        private static bool IsInvalidSsl2Hello(byte[] buffer)
        {
            if (buffer.Length < 6)
                return false;
            var offset = (buffer[0] & 0x80) != 0 ? 2 : 3;
            return buffer[offset] != 1
                || buffer[offset + 1] != 3
                || (buffer[offset + 2] != 0 && buffer[offset + 2] != 1);
        }

        private static bool IsSsl2HelloComplete(byte[] buffer)
        {
            return buffer.Length >= 3
                && (
                    (buffer[0] & 0x80) != 0
                        ? buffer.Length == (((buffer[0] & 0x7f) << 8) | (buffer[1] + 2))
                        : buffer.Length == (((buffer[0] & 0x3f) << 8) | (buffer[1] + 3))
                );
        }

        private static byte[] ExtractSsl2Content(byte[] buffer)
        {
            var ret =
                (buffer[0] & 0x80) != 0
                    ? (new byte[buffer.Length - 2])
                    : (new byte[buffer.Length - 3]);
            Array.Copy(buffer, buffer.Length - ret.Length, ret, 0, ret.Length);
            return ret;
        }

        private static ProtocolVersion ExtractSsl2Version(byte[] buffer)
        {
            if ((buffer[0] & 0x80) != 0)
                return new ProtocolVersion(buffer[3], buffer[4]); // no padding
            return new ProtocolVersion(buffer[4], buffer[5]); // padding
        }

        private CompatibilityResult ProcessClientHello(byte[] bytes, int offset, int size)
        {
            var temp = new byte[m_Buffer.Length + size];
            Array.Copy(m_Buffer, 0, temp, 0, m_Buffer.Length);
            Array.Copy(bytes, offset, temp, m_Buffer.Length, size);
            if (IsInvalidSsl3Hello(temp) && IsInvalidSsl2Hello(temp)) // SSL2 hello
                throw new SslException(
                    AlertDescription.HandshakeFailure,
                    "The client hello message uses a protocol that was not recognized."
                );
            if (
                m_Buffer.Length + bytes.Length < 11
                || (IsInvalidSsl3Hello(temp) && !IsSsl2HelloComplete(temp))
            )
            { // not enough bytes
                m_Buffer = temp;
                return new CompatibilityResult(
                    null,
                    new SslRecordStatus(SslStatus.MessageIncomplete, null, null)
                );
            }
            var pv = !IsInvalidSsl3Hello(temp)
                ? new ProtocolVersion(temp[9], temp[10])
                : ExtractSsl2Version(temp);
            if (pv.GetVersionInt() > m_MaxVersion.GetVersionInt())
                pv = m_MaxVersion;
            if (SupportsProtocol(m_Options.Protocol, pv))
            {
                if (m_MinLayer.HandshakeLayer.GetVersion().GetVersionInt() != pv.GetVersionInt())
                {
                    m_MinLayer.HandshakeLayer =
                        pv.GetVersionInt() == 30
                            ? new Ssl3ServerHandshakeLayer(m_MinLayer.HandshakeLayer)
                            : new Tls1ServerHandshakeLayer(m_MinLayer.HandshakeLayer);
                }
                return !IsInvalidSsl3Hello(temp)
                    ? new CompatibilityResult(
                        m_MinLayer,
                        m_MinLayer.ProcessBytes(temp, 0, temp.Length)
                    )
                    : new CompatibilityResult(
                        m_MinLayer,
                        m_MinLayer.ProcessSsl2Hello(ExtractSsl2Content(temp))
                    );
            }
            else
            {
                throw new SslException(
                    AlertDescription.HandshakeFailure,
                    "The client and server could not agree on the protocol version to use."
                );
            }
        }

        public static bool SupportsSsl3(SecureProtocol protocol)
        {
            return ((int)protocol & (int)SecureProtocol.Ssl3) != 0;
        }

        public static bool SupportsTls1(SecureProtocol protocol)
        {
            return ((int)protocol & (int)SecureProtocol.Tls1) != 0;
        }

        public static bool SupportsTls1_1(SecureProtocol protocol)
        {
            return ((int)protocol & (int)SecureProtocol.Tls1_1) != 0;
        }

        public static bool SupportsProtocol(SecureProtocol protocol, ProtocolVersion pv)
        {
            return pv.GetVersionInt() switch
            {
                30 => SupportsSsl3(protocol),
                31 => SupportsTls1(protocol),
                32 => SupportsTls1_1(protocol),
                _ => false,
            };
        }

        public static ProtocolVersion GetMinProtocol(SecureProtocol protocol)
        {
            if (SupportsSsl3(protocol))
                return new ProtocolVersion(3, 0);
            if (SupportsTls1(protocol))
                return new ProtocolVersion(3, 1);
            if (SupportsTls1_1(protocol))
                return new ProtocolVersion(3, 2);

            throw new SslException(
                AlertDescription.ProtocolVersion,
                "[CompatibilityLayer] - GetMinProtocol: No supported protocols found."
            );
        }

        public static ProtocolVersion GetMaxProtocol(SecureProtocol protocol)
        {
            if (SupportsTls1_1(protocol))
                return new ProtocolVersion(3, 2);
            if (SupportsTls1(protocol))
                return new ProtocolVersion(3, 1);
            if (SupportsSsl3(protocol))
                return new ProtocolVersion(3, 0);

            throw new SslException(
                AlertDescription.ProtocolVersion,
                "[CompatibilityLayer] - GetMaxProtocol: No supported protocols found."
            );
        }

        private byte[] m_Hello;
        private ProtocolVersion m_MinVersion;
        private ProtocolVersion m_MaxVersion;
        private readonly RecordLayer m_MinLayer;
        private readonly SecurityOptions m_Options;
        private byte[] m_Buffer;
    }
}
