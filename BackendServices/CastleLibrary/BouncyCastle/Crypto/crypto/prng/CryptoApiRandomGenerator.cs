using System;
using System.Security.Cryptography;

namespace Org.BouncyCastle.Crypto.Prng
{
    /// <summary>
    /// Uses RandomNumberGenerator.Create() to get randomness generator
    /// </summary>
    public sealed class CryptoApiRandomGenerator : IRandomGenerator, IDisposable
    {
        private readonly RandomNumberGenerator m_randomNumberGenerator;

        public CryptoApiRandomGenerator()
            : this(RandomNumberGenerator.Create()) { }

        public CryptoApiRandomGenerator(RandomNumberGenerator randomNumberGenerator)
        {
            m_randomNumberGenerator =
                randomNumberGenerator
                ?? throw new ArgumentNullException(nameof(randomNumberGenerator));
        }

        #region IRandomGenerator Members

        public void AddSeedMaterial(byte[] seed)
        {
            // We don't care about the seed
        }

        public void AddSeedMaterial(ReadOnlySpan<byte> inSeed)
        {
            // We don't care about the seed
        }

        public void AddSeedMaterial(long seed)
        {
            // We don't care about the seed
        }

        public void NextBytes(byte[] bytes)
        {
            m_randomNumberGenerator.GetBytes(bytes);
        }

        public void NextBytes(byte[] bytes, int start, int len)
        {
            m_randomNumberGenerator.GetBytes(bytes, start, len);
        }

        public void NextBytes(Span<byte> bytes)
        {
            m_randomNumberGenerator.GetBytes(bytes);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_randomNumberGenerator.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
