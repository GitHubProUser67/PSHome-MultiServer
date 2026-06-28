// SHA-224 authentication provider.
// Copyright (C) 2008-2010 Malcolm Crowe, Lex Li, and other contributors.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace CastleLibrary.BouncyCastle.Lextm
{
    /// <summary>
    /// Authentication provider using SHA-224.
    /// </summary>
    /// <remarks>Defined in https://tools.ietf.org/html/rfc7630#page-3.</remarks>
    public sealed class BouncyCastleSHA224AuthenticationProvider : IAuthenticationProvider
    {
        private const int Sha224KeyCacheCapacity = 100;
        private static readonly CryptoKeyCache Sha224KeyCache = new(Sha224KeyCacheCapacity);
        private static readonly Lock Sha224KeyCacheLock = new();

        private readonly byte[] _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="SHA224AuthenticationProvider"/> class.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        public BouncyCastleSHA224AuthenticationProvider(OctetString phrase)
        {
            ArgumentNullException.ThrowIfNull(phrase);

            _password = phrase.GetRaw();
        }

        #region IAuthenticationProvider Members
        /// <summary>
        /// Passwords to key.
        /// </summary>
        /// <param name="password">The user password.</param>
        /// <param name="engineId">The engine ID.</param>
        /// <returns></returns>
        public byte[] PasswordToKey(byte[] password, byte[] engineId)
        {
            // key length has to be at least 8 bytes long (RFC3414)
            ArgumentNullException.ThrowIfNull(password);

            ArgumentNullException.ThrowIfNull(engineId);

            if (password.Length < 8)
            {
                throw new ArgumentException(
                    $"Secret key is too short. Must be >= 8. Current: {password.Length}.",
                    nameof(password)
                );
            }

            lock (Sha224KeyCacheLock)
            {
                if (Sha224KeyCache.TryGetCachedValue(password, engineId, out var cachedKey))
                    return cachedKey;

                const int bufferSize = 1048576; // 1 Megabyte

                var sha = new Sha224Digest();
                {
                    var passwordIndex = 0;
                    var count = 0;
                    /* Use while loop until we've done 1 Megabyte */
                    var sourceBuffer = new byte[bufferSize];
                    var buf = new byte[64];
                    while (count < bufferSize)
                    {
                        for (var i = 0; i < 64; ++i)
                            // Take the next octet of the password, wrapping
                            // to the beginning of the password as necessary.
                            buf[i] = password[passwordIndex++ % password.Length];

                        Buffer.BlockCopy(buf, 0, sourceBuffer, count, buf.Length);
                        count += 64;
                    }

                    var digest = new byte[sha.GetDigestSize()];
                    sha.BlockUpdate(sourceBuffer, 0, sourceBuffer.Length);
                    sha.DoFinal(digest, 0);

                    using (var buffer = new MemoryStream())
                    {
                        buffer.Write(digest, 0, digest.Length);
                        buffer.Write(engineId, 0, engineId.Length);
                        buffer.Write(digest, 0, digest.Length);
                        var input = buffer.ToArray();
                        sha.BlockUpdate(input, 0, input.Length);
                        sha.DoFinal(digest, 0);
                        //Value not in cache compute and cache the value
                        Sha224KeyCache.AddValueToCache(password, engineId, digest);
                        return digest;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the clean digest.
        /// </summary>
        /// <value>The clean digest.</value>
        public OctetString CleanDigest
        {
            get { return new OctetString(new byte[DigestLength]); }
        }

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="header">The header.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="data">The scope bytes.</param>
        /// <param name="privacy">The privacy provider.</param>
        /// <param name="length">The length bytes.</param>
        /// <returns></returns>
        public OctetString ComputeHash(
            VersionCode version,
            ISegment header,
            SecurityParameters parameters,
            ISnmpData data,
            IPrivacyProvider privacy,
            byte[] length
        )
        {
            ArgumentNullException.ThrowIfNull(header);

            ArgumentNullException.ThrowIfNull(parameters);

            ArgumentNullException.ThrowIfNull(data);

            ArgumentNullException.ThrowIfNull(privacy);

            var key = PasswordToKey(_password, parameters.EngineId.GetRaw());
            var sha224 = new HMac(new Sha224Digest());
            {
                sha224.Init(new KeyParameter(key));
                var message = ByteTool
                    .PackMessage(length, version, header, parameters, data)
                    .ToBytes();
                var hash = new byte[sha224.GetMacSize()];
                sha224.BlockUpdate(message, 0, message.Length);
                sha224.DoFinal(hash, 0);

                var result = new byte[DigestLength];
                Buffer.BlockCopy(hash, 0, result, 0, result.Length);
                return new OctetString(result);
            }
        }

        public int DigestLength => 32;

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "SHA-224 authentication provider";
        }
    }
}
