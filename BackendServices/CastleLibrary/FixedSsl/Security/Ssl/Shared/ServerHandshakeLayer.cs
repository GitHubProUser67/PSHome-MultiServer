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

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CastleLibrary.FixedSsl.Crypto;
using CastleLibrary.FixedSsl.Security.Certificates;

namespace CastleLibrary.FixedSsl.Security.Ssl.Shared
{
    /*
      Client                                               Server

      ClientHello                  -------->
                                                      ServerHello
                                                     Certificate*
                                               ServerKeyExchange*
                                              CertificateRequest*
                                   <--------      ServerHelloDone
      Certificate*
      ClientKeyExchange
      CertificateVerify*
      [ChangeCipherSpec]
      Finished                     -------->
                                               [ChangeCipherSpec]
                                   <--------             Finished
      Application Data             <------->     Application Data
    */
    internal abstract class ServerHandshakeLayer : HandshakeLayer
    {
        public ServerHandshakeLayer(RecordLayer recordLayer, SecurityOptions options)
            : base(recordLayer, options) { }

        public ServerHandshakeLayer(HandshakeLayer handshakeLayer)
            : base(handshakeLayer)
        {
            m_MaxClientVersion = ((ServerHandshakeLayer)handshakeLayer).m_MaxClientVersion;
        }

        protected override SslHandshakeStatus ProcessMessage(HandshakeMessage message)
        { // throws SslExceptions
            SslHandshakeStatus ret;
            switch (message.type)
            {
                case HandshakeType.ClientHello:
                    ret = ProcessClientHello(message);
                    break;
                case HandshakeType.Certificate: // optional
                    ret = ProcessCertificate(message, false);
                    break;
                case HandshakeType.ClientKeyExchange:
                    ret = ProcessClientKeyExchange(message);
                    break;
                case HandshakeType.CertificateVerify: // optional
                    ret = ProcessCertificateVerify(message);
                    break;
                case HandshakeType.Finished:
                    ret = ProcessFinished(message);
                    break;
                default:
                    throw new SslException(
                        AlertDescription.UnexpectedMessage,
                        "The received message was not expected from a client."
                    );
            }
            return ret;
        }

        protected SslHandshakeStatus ProcessClientHello(HandshakeMessage message)
        {
            if (m_State != HandshakeType.Nothing && m_State != HandshakeType.Finished)
                throw new SslException(
                    AlertDescription.UnexpectedMessage,
                    "ClientHello message must be the first message or must be preceded by a Finished message."
                );
            m_IsNegotiating = true;
            UpdateHashes(message, HashUpdate.All); // input message
            // process ClientHello
            var pv = new ProtocolVersion(message.fragment[0], message.fragment[1]);
            m_MaxClientVersion = pv;
            if (
                CompatibilityLayer.SupportsProtocol(m_Options.Protocol, pv)
                && pv.GetVersionInt() != GetVersion().GetVersionInt()
            )
                throw new SslException(
                    AlertDescription.IllegalParameter,
                    "Unknown protocol version of the client."
                );
            try
            {
                // extract the time from the client [== 1 uint]
                m_ClientTime = new byte[4];
                Array.Copy(message.fragment, 2, m_ClientTime, 0, 4);
                // extract the random bytes [== 28 bytes]
                m_ClientRandom = new byte[28];
                Array.Copy(message.fragment, 6, m_ClientRandom, 0, 28);
                // extact the session ID [== 0..32 bytes]
                int length = message.fragment[34];
                if (length > 32)
                    throw new SslException(
                        AlertDescription.IllegalParameter,
                        "The length of the SessionID cannot be more than 32 bytes."
                    );
                m_SessionID = new byte[length];
                Array.Copy(message.fragment, 35, m_SessionID, 0, length);
                // extract the available cipher suites
                length += 35;
                var ciphers_size = (message.fragment[length] * 256) + message.fragment[length + 1];
                if (ciphers_size < 2 || ciphers_size % 2 != 0)
                    throw new SslException(
                        AlertDescription.IllegalParameter,
                        "The number of ciphers is invalid -or- the cipher length is not even."
                    );
                var ciphers = new byte[ciphers_size];
                Array.Copy(message.fragment, length + 2, ciphers, 0, ciphers_size);
                m_EncryptionScheme = CipherSuites.GetCipherSuiteAlgorithm(
                    ciphers,
                    m_Options.AllowedAlgorithms
                );
                // extract the available compression algorithms
                length += ciphers_size + 2;
                int compressors_size = message.fragment[length];
                if (compressors_size == 0)
                    throw new SslException(
                        AlertDescription.IllegalParameter,
                        "No compressor specified."
                    );
                var compressors = new byte[compressors_size];
                Array.Copy(message.fragment, length + 1, compressors, 0, compressors_size);
                m_CompressionMethod = CompressionAlgorithm.GetCompressionAlgorithm(
                    compressors,
                    m_Options.AllowedAlgorithms
                );
            }
            catch (Exception e)
            {
                throw new SslException(
                    e,
                    AlertDescription.InternalError,
                    "The message is invalid."
                );
            }
            // create reply
            return GetClientHelloResult();
        }

        protected SslHandshakeStatus GetClientHelloResult()
        {
            var retMessage = new MemoryStream();
            var ret = new SslHandshakeStatus();
            HandshakeMessage temp;
            byte[] bytes;
            // ServerHello message
            temp = new HandshakeMessage(HandshakeType.ServerHello, new byte[38]);
            m_ServerTime = GetUnixTime();
            m_ServerRandom = new byte[28];
            RandomNumberGenerator.Fill(m_ServerRandom);
            temp.fragment[0] = GetVersion().major;
            temp.fragment[1] = GetVersion().minor;
            Array.Copy(m_ServerTime, 0, temp.fragment, 2, 4);
            Array.Copy(m_ServerRandom, 0, temp.fragment, 6, 28);
            temp.fragment[34] = 0; // do not resume a session, and do not let the other side cache it
            Array.Copy(
                CipherSuites.GetCipherAlgorithmBytes(m_EncryptionScheme),
                0,
                temp.fragment,
                35,
                2
            );
            temp.fragment[37] = CompressionAlgorithm.GetAlgorithmByte(m_CompressionMethod);
            bytes = temp.ToBytes();
            retMessage.Write(bytes, 0, bytes.Length);
            // Certificate message
            var certs = GetCertificateList(m_Options.Certificate);
            temp.type = HandshakeType.Certificate;
            temp.fragment = certs;
            bytes = temp.ToBytes();
            retMessage.Write(bytes, 0, bytes.Length);
            // ServerKeyExchange message [optional] => only with RSA_EXPORT and public key > 512 bits
            if (
                m_Options.Certificate.GetPublicKeyLength() > 512
                && CipherSuites.GetCipherDefinition(m_EncryptionScheme).Exportable
            )
            {
                var kes = new MemoryStream();
                var mscsp = new MD5SHA1CryptoServiceProvider();
                // hash the client and server random values
                mscsp.TransformBlock(m_ClientTime, 0, 4, m_ClientTime, 0);
                mscsp.TransformBlock(m_ClientRandom, 0, 28, m_ClientRandom, 0);
                mscsp.TransformBlock(m_ServerTime, 0, 4, m_ServerTime, 0);
                mscsp.TransformBlock(m_ServerRandom, 0, 28, m_ServerRandom, 0);
                // create a new 512 bit RSA key
                m_KeyCipher = new RSACryptoServiceProviderWrapper(
                    new RSACryptoServiceProvider(512)
                );
                var p = m_KeyCipher.RsaCsp.ExportParameters(false);
                // write the key parameters to the output stream
                bytes = new byte[]
                {
                    (byte)(p.Modulus.Length / 256),
                    (byte)(p.Modulus.Length % 256),
                };
                kes.Write(bytes, 0, 2);
                kes.Write(p.Modulus, 0, p.Modulus.Length);
                mscsp.TransformBlock(bytes, 0, 2, bytes, 0);
                mscsp.TransformBlock(p.Modulus, 0, p.Modulus.Length, p.Modulus, 0);
                bytes = new byte[]
                {
                    (byte)(p.Exponent.Length / 256),
                    (byte)(p.Exponent.Length % 256),
                };
                kes.Write(bytes, 0, 2);
                kes.Write(p.Exponent, 0, p.Exponent.Length);
                mscsp.TransformBlock(bytes, 0, 2, bytes, 0);
                mscsp.TransformFinalBlock(p.Exponent, 0, p.Exponent.Length);
                // create signature
                bytes = mscsp.CreateSignature(m_Options.Certificate);
                kes.Write(
                    new byte[] { (byte)(bytes.Length / 256), (byte)(bytes.Length % 256) },
                    0,
                    2
                );
                kes.Write(bytes, 0, bytes.Length);
                // write to output
                temp.type = HandshakeType.ServerKeyExchange;
                temp.fragment = kes.ToArray();
                bytes = temp.ToBytes();
                retMessage.Write(bytes, 0, bytes.Length);
                kes.Close();
            }
            else
                m_KeyCipher = new RSACryptoServiceProviderWrapper(m_Options.Certificate.PrivateKey);
            // CertificateRequest message [optional]
            if (m_MutualAuthentication)
            {
                bytes = GetDistinguishedNames();
                if (bytes.Length != 0)
                { // make sure at least one certificate is returned
                    temp.type = HandshakeType.CertificateRequest;
                    temp.fragment = new byte[bytes.Length + 4];
                    temp.fragment[0] = 1; // one certificate type supported
                    temp.fragment[1] = 1; // cert type RSA
                    temp.fragment[2] = (byte)(bytes.Length / 256);
                    temp.fragment[3] = (byte)(bytes.Length % 256);
                    Array.Copy(bytes, 0, temp.fragment, 4, bytes.Length);
                    bytes = temp.ToBytes();
                    retMessage.Write(bytes, 0, bytes.Length);
                }
            }
            // ServerHelloDone message
            temp.type = HandshakeType.ServerHelloDone;
            temp.fragment = Array.Empty<byte>();
            bytes = temp.ToBytes();
            retMessage.Write(bytes, 0, bytes.Length);
            // final adjustments
            ret.Status = SslStatus.ContinueNeeded;
            ret.Message = retMessage.ToArray();
            retMessage.Close();
            UpdateHashes(ret.Message, HashUpdate.All); // output message
            ret.Message = m_RecordLayer.EncryptBytes(
                ret.Message,
                0,
                ret.Message.Length,
                ContentType.Handshake
            );
            return ret;
        }

        protected static byte[] GetDistinguishedNames()
        {
            using (var ms = new MemoryStream())
            {
                var store = new X509Store(
                    StoreName.Root,
                    System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser
                );
                store.Open(OpenFlags.ReadOnly);

                foreach (var cert in store.Certificates)
                {
                    if (cert.NotBefore <= DateTime.Now && cert.NotAfter >= DateTime.Now)
                    {
                        // KeyUsage check for Certificate Signing
                        if (
                            cert
                                .Extensions.OfType<X509KeyUsageExtension>()
                                .Any(k => (k.KeyUsages & X509KeyUsageFlags.KeyCertSign) != 0)
                        )
                        {
                            var dnBytes = System.Text.Encoding.ASCII.GetBytes(cert.Subject);

                            if (ms.Length + dnBytes.Length + 2 < 65536)
                            {
                                ms.WriteByte((byte)(dnBytes.Length / 256));
                                ms.WriteByte((byte)(dnBytes.Length % 256));
                                ms.Write(dnBytes, 0, dnBytes.Length);
                            }
                        }
                    }
                }

                store.Close();
                return ms.ToArray();
            }
        }

        protected static byte[] GetCertificateList(Certificate certificate)
        {
            var certs = new Certificate[] { certificate };
            var cert_bytes = new byte[certs.Length][];
            var size = 0;
            for (var i = 0; i < certs.Length; i++)
            {
                cert_bytes[i] = certs[i].ToCerBuffer();
                size += cert_bytes[i].Length + 3;
            }
            using (var ret = new MemoryStream(size + (3 * certs.Length) + 3))
            {
                // write length of certificate list
                ret.WriteByte((byte)(size / 65536));
                ret.WriteByte((byte)(size % 65536 / 256));
                ret.WriteByte((byte)(size % 256));
                for (var i = 0; i < cert_bytes.Length; i++)
                {
                    // write the length of the certificate
                    size = cert_bytes[i].Length;
                    ret.WriteByte((byte)(size / 65536)); // write length of certificates
                    ret.WriteByte((byte)(size % 65536 / 256));
                    ret.WriteByte((byte)(size % 256));
                    // write the certificate
                    ret.Write(cert_bytes[i], 0, size);
                }
                return ret.ToArray();
            }
        }

        protected SslHandshakeStatus ProcessClientKeyExchange(HandshakeMessage message)
        {
            if (
                !(
                    m_MutualAuthentication
                        ? m_State == HandshakeType.Certificate
                        : m_State == HandshakeType.ClientHello
                )
            )
                throw new SslException(
                    AlertDescription.UnexpectedMessage,
                    "ClientKeyExchange message must be preceded by a ClientHello or Certificate message."
                );
            byte[] preMasterSecret;
            UpdateHashes(message, HashUpdate.All); // input message
            try
            {
                if (message.fragment.Length % 8 == 2)
                { // check whether the length is prepended or not
                    if (
                        (message.fragment[0] * 256) + message.fragment[1]
                        != message.fragment.Length - 2
                    )
                        throw new SslException(
                            AlertDescription.DecodeError,
                            "Invalid ClientKeyExchange message."
                        );
                    preMasterSecret = new byte[message.fragment.Length - 2];
                    Array.Copy(message.fragment, 2, preMasterSecret, 0, preMasterSecret.Length);
                    message.fragment = preMasterSecret;
                }
                var df = new RSAKeyTransform(m_KeyCipher);
                preMasterSecret = df.DecryptKeyExchange(message.fragment);
                if (preMasterSecret.Length != 48)
                    throw new SslException(AlertDescription.IllegalParameter, "Invalid message.");
                if (((int)m_Options.Flags & (int)SecurityFlags.IgnoreMaxProtocol) == 0)
                {
                    if (
                        preMasterSecret[0] != m_MaxClientVersion.major
                        || preMasterSecret[1] != m_MaxClientVersion.minor
                    )
                        throw new SslException(
                            AlertDescription.IllegalParameter,
                            "Version rollback detected."
                        );
                }
                else
                {
                    if (
                        preMasterSecret[0] != 3
                        || (preMasterSecret[1] != 0 && preMasterSecret[1] != 1)
                    )
                        throw new SslException(
                            AlertDescription.IllegalParameter,
                            "Invalid protocol version detected."
                        );
                }
                m_KeyCipher.Clear();
                m_KeyCipher = null;
            }
            catch
            {
                // this is to avoid RSA PKCS#1 padding attacks
                // and the Klima-Pokorny-Rosa attack on RSA in SSL/TLS
                preMasterSecret = new byte[48];
                RandomNumberGenerator.Fill(preMasterSecret);
            }
            GenerateCiphers(preMasterSecret);
            return new SslHandshakeStatus(SslStatus.MessageIncomplete, null);
        }

        protected SslHandshakeStatus ProcessCertificateVerify(HandshakeMessage message)
        {
            if (m_State != HandshakeType.ClientKeyExchange)
                throw new SslException(
                    AlertDescription.UnexpectedMessage,
                    "CertificateVerify message must be preceded by a ClientKeyExchange message."
                );
            UpdateHashes(message, HashUpdate.LocalRemote);
            byte[] signature;
            if (message.fragment.Length % 8 == 2)
            { // check whether the length is prepended or not
                if (
                    (message.fragment[0] * 256) + message.fragment[1]
                    != message.fragment.Length - 2
                )
                    throw new SslException(
                        AlertDescription.DecodeError,
                        "Invalid CertificateVerify message."
                    );
                signature = new byte[message.fragment.Length - 2];
                Array.Copy(message.fragment, 2, signature, 0, signature.Length);
            }
            else
                signature = message.fragment;
            m_CertSignHash.MasterKey = m_MasterSecret;
            m_CertSignHash.TransformFinalBlock(signature, 0, 0); // finalize hash
            return !m_CertSignHash.VerifySignature(m_RemoteCertificate, signature)
                ? throw new SslException(
                    AlertDescription.CertificateUnknown,
                    "The signature in the CertificateVerify message is invalid."
                )
                : new SslHandshakeStatus(SslStatus.MessageIncomplete, null);
        }

        protected SslHandshakeStatus ProcessFinished(HandshakeMessage message)
        {
            if (m_State != HandshakeType.ChangeCipherSpec)
                throw new SslException(
                    AlertDescription.UnexpectedMessage,
                    "Finished message must be preceded by a ChangeCipherSpec message."
                );
            byte[] temp;
            // check hash received from client
            VerifyFinishedMessage(message.fragment);
            // send ChangeCipherSpec
            UpdateHashes(message, HashUpdate.Local);
            var ms = new MemoryStream();
            temp = m_RecordLayer.EncryptBytes(new byte[] { 1 }, 0, 1, ContentType.ChangeCipherSpec);
            ms.Write(temp, 0, temp.Length);
            m_RecordLayer.ChangeLocalState(
                null,
                m_CipherSuite.Encryptor,
                m_CipherSuite.LocalHasher
            );
            // send Finished message
            temp = GetFinishedMessage();
            temp = m_RecordLayer.EncryptBytes(temp, 0, temp.Length, ContentType.Handshake);
            ms.Write(temp, 0, temp.Length);
            m_State = HandshakeType.Nothing;
            // send empty record [http://www.openssl.org/~bodo/tls-cbc.txt]
            if (m_CipherSuite.Encryptor.OutputBlockSize != 1)
            { // is bulk cipher?
                if (((int)m_Options.Flags & (int)SecurityFlags.DontSendEmptyRecord) == 0)
                {
                    var empty = m_RecordLayer.EncryptBytes(
                        Array.Empty<byte>(),
                        0,
                        0,
                        ContentType.ApplicationData
                    );
                    ms.Write(empty, 0, empty.Length);
                }
            }
            // finalize
            var ret = ms.ToArray();
            ms.Close();
            m_IsNegotiating = false;
            ClearHandshakeStructures();
            return new SslHandshakeStatus(SslStatus.OK, ret);
        }

        protected override SslHandshakeStatus ProcessChangeCipherSpec(RecordMessage message)
        {
            if (message.length != 1 || message.fragment[0] != 1)
                throw new SslException(
                    AlertDescription.IllegalParameter,
                    "The ChangeCipherSpec message was invalid."
                );
            if (
                m_State == HandshakeType.ClientKeyExchange
                || m_State == HandshakeType.CertificateVerify
            )
            {
                m_RecordLayer.ChangeRemoteState(
                    null,
                    m_CipherSuite.Decryptor,
                    m_CipherSuite.RemoteHasher
                );
                return new SslHandshakeStatus(SslStatus.MessageIncomplete, null); // needs a finished message
            }
            throw new SslException(
                AlertDescription.UnexpectedMessage,
                "ChangeCipherSpec message must be preceded by a ClientKeyExchange or CertificateVerify message."
            );
        }

        protected override byte[] GetRenegotiateBytes()
        {
            if (IsNegotiating())
                return null;
            var hm = new HandshakeMessage(HandshakeType.HelloRequest, Array.Empty<byte>());
            return m_RecordLayer.EncryptBytes(hm.ToBytes(), 0, 4, ContentType.Handshake);
        }

        protected override byte[] GetClientHello()
        {
            throw new SslException(
                AlertDescription.InternalError,
                "This is a server socket; it cannot send client hello messages"
            );
        }

        // Thanks to Brandon for notifying us about a bug in this method
        public override SslHandshakeStatus ProcessSsl2Hello(byte[] hello)
        {
            if (m_State != HandshakeType.Nothing)
                throw new SslException(
                    AlertDescription.UnexpectedMessage,
                    "SSL2 ClientHello message must be the first message."
                );
            m_IsNegotiating = true;
            m_State = HandshakeType.ClientHello;
            UpdateHashes(hello, HashUpdate.All); // input message
            // process ClientHello
            var pv = new ProtocolVersion(hello[1], hello[2]);
            m_MaxClientVersion = pv;
            if (
                CompatibilityLayer.SupportsProtocol(m_Options.Protocol, pv)
                && pv.GetVersionInt() != GetVersion().GetVersionInt()
            )
                throw new SslException(
                    AlertDescription.IllegalParameter,
                    "Unknown protocol version of the client."
                );
            var csl = (hello[3] * 256) + hello[4]; // cipher spec length
            var sidl = (hello[5] * 256) + hello[6]; // session id length
            var cl = (hello[7] * 256) + hello[8]; // challenge length
            // process ciphers
            var ciphers = new byte[csl / 3 * 2];
            var offset = 10;
            for (var i = 0; i < ciphers.Length; i += 2)
            {
                Array.Copy(hello, offset, ciphers, i, 2);
                offset += 3;
            }
            m_EncryptionScheme = CipherSuites.GetCipherSuiteAlgorithm(
                ciphers,
                m_Options.AllowedAlgorithms
            );
            // process session id
            m_SessionID = new byte[sidl];
            Array.Copy(hello, 9 + csl, m_SessionID, 0, sidl);
            // process random data [challenge]
            m_ClientTime = new byte[4];
            m_ClientRandom = new byte[28];
            if (cl <= 28)
                Array.Copy(hello, 9 + csl + sidl, m_ClientRandom, m_ClientRandom.Length - cl, cl);
            else
            {
                Array.Copy(hello, 9 + csl + sidl + (cl - 28), m_ClientRandom, 0, 28);
                Array.Copy(hello, 9 + csl + sidl, m_ClientTime, 4 - (cl - 28), cl - 28);
            }
            m_CompressionMethod = SslAlgorithms.NULL_COMPRESSION;
            return GetClientHelloResult();
        }

        protected ProtocolVersion m_MaxClientVersion;
    }
}
