/*
 *   Mentalis.org Security Library
 *
 *     Copyright ï¿½ 2002-2005, The Mentalis.org Team
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
using CastleLibrary.FixedSsl.Crypto.Mono.Security;
using CastleLibrary.FixedSsl.Security.Certificates;
using CastleLibrary.FixedSsl.Security.Ssl.Ssl3;

namespace CastleLibrary.FixedSsl.Security.Ssl.Shared
{
    internal sealed class MD5SHA1CryptoServiceProvider : HashAlgorithm
    {
        public MD5SHA1CryptoServiceProvider()
        {
            HashSizeValue = 36 * 8; // In bits (required for Mono)
            m_MD5 = MD5.Create();
            m_SHA1 = SHA1.Create();
        }

        protected override void Dispose(bool disposing)
        {
            m_MD5.Clear();
            m_SHA1.Clear();
            if (m_MasterKey != null)
                Array.Clear(m_MasterKey, 0, m_MasterKey.Length);
            try
            {
                GC.SuppressFinalize(this);
            }
            catch { }
        }

        public override void Initialize()
        {
            m_MD5.Initialize();
            m_SHA1.Initialize();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            m_MD5.TransformBlock(array, ibStart, cbSize, array, ibStart);
            m_SHA1.TransformBlock(array, ibStart, cbSize, array, ibStart);
        }

        public SecureProtocol Protocol
        {
            get { return m_Protocol; }
            set { m_Protocol = value; }
        }
        public byte[] MasterKey
        {
            get { return m_MasterKey; }
            set { m_MasterKey = (byte[])value.Clone(); }
        }

        protected override byte[] HashFinal()
        {
            if (m_Protocol == SecureProtocol.Ssl3)
            {
                m_MD5 = new Ssl3HandshakeMac(HashType.MD5, m_MD5, m_MasterKey);
                m_SHA1 = new Ssl3HandshakeMac(HashType.SHA1, m_SHA1, m_MasterKey);
            }
            var hash = new byte[36];
            m_MD5.TransformFinalBlock(hash, 0, 0);
            m_SHA1.TransformFinalBlock(hash, 0, 0);
            Array.Copy(m_MD5.Hash, 0, hash, 0, 16);
            Array.Copy(m_SHA1.Hash, 0, hash, 16, 20);
            return hash;
        }

        public bool VerifySignature(Certificate cert, byte[] signature)
        {
            return VerifySignature(cert, signature, Hash);
        }

        private bool VerifySignature(Certificate cert, byte[] signature, byte[] hash)
        {
            return PKCS1.Verify_v15(cert.PublicKey, this, hash, signature);
        }

        public byte[] CreateSignature(Certificate cert)
        {
            return CreateSignature(cert, Hash);
        }

        private byte[] CreateSignature(Certificate cert, byte[] hash)
        {
            return PKCS1.Sign_v15(cert.PrivateKey, this, hash);
        }

        ~MD5SHA1CryptoServiceProvider()
        {
            Clear();
        }

        private HashAlgorithm m_MD5;
        private HashAlgorithm m_SHA1;
        private SecureProtocol m_Protocol;
        private byte[] m_MasterKey;
    }
}
