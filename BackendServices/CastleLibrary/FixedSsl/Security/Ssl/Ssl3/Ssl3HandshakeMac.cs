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

namespace CastleLibrary.FixedSsl.Security.Ssl.Ssl3
{
    internal sealed class Ssl3HandshakeMac : KeyedHashAlgorithm
    {
        // hash(master_secret + pad2 + hash(handshake_messages +
        //      Sender + master_secret + pad1));
        public Ssl3HandshakeMac(HashType hash)
            : this(hash, null) { }

        public Ssl3HandshakeMac(HashType hashType, byte[] rgbKey)
        {
            ArgumentNullException.ThrowIfNull(rgbKey);
            if (hashType == HashType.MD5)
            {
                m_HashAlgorithm = MD5.Create();
                m_PadSize = 48;
            }
            else
            { // SHA1
                m_HashAlgorithm = SHA1.Create();
                m_PadSize = 40;
            }
            KeyValue = (byte[])rgbKey.Clone();
            m_IsDisposed = false;
            Initialize();
        }

        public Ssl3HandshakeMac(HashType hashType, HashAlgorithm hash, byte[] rgbKey)
        {
            ArgumentNullException.ThrowIfNull(rgbKey);
            m_PadSize = hashType == HashType.MD5 ? 48 : 40;
            m_HashAlgorithm = hash;
            KeyValue = (byte[])rgbKey.Clone();
            m_IsDisposed = false;
        }

        public override void Initialize()
        {
            ObjectDisposedException.ThrowIf(m_IsDisposed, this);
            m_HashAlgorithm.Initialize();
            State = 0;
        }

        protected override void HashCore(byte[] rgb, int ib, int cb)
        {
            ObjectDisposedException.ThrowIf(m_IsDisposed, this);
            m_HashAlgorithm.TransformBlock(rgb, ib, cb, rgb, ib);
        }

        protected override byte[] HashFinal()
        {
            ObjectDisposedException.ThrowIf(m_IsDisposed, this);
            m_HashAlgorithm.TransformBlock(KeyValue, 0, KeyValue.Length, KeyValue, 0);
            var padding = new byte[m_PadSize];
            for (var i = 0; i < padding.Length; i++)
                padding[i] = 0x36;
            m_HashAlgorithm.TransformFinalBlock(padding, 0, padding.Length); // finalize inner hash
            var dataHash = m_HashAlgorithm.Hash;
            for (var i = 0; i < padding.Length; i++)
                padding[i] = 0x5C;
            m_HashAlgorithm.Initialize();
            m_HashAlgorithm.TransformBlock(Key, 0, Key.Length, Key, 0);
            m_HashAlgorithm.TransformBlock(padding, 0, padding.Length, padding, 0);
            m_HashAlgorithm.TransformFinalBlock(dataHash, 0, dataHash.Length);
            return m_HashAlgorithm.Hash;
        }

        public override int HashSize
        {
            get { return m_HashAlgorithm.HashSize; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_IsDisposed = true;
            m_HashAlgorithm.Clear();
            try
            {
                GC.SuppressFinalize(this);
            }
            catch { }
        }

        ~Ssl3HandshakeMac()
        {
            m_HashAlgorithm.Clear();
        }

        private readonly HashAlgorithm m_HashAlgorithm;
        private bool m_IsDisposed;
        private readonly int m_PadSize;
    }
}
