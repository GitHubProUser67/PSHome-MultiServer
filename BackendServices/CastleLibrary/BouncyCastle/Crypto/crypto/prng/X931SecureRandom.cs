using System;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Prng
{
    public class X931SecureRandom : SecureRandom
    {
        private readonly bool mPredictionResistant;
        private readonly SecureRandom mRandomSource;
        private readonly X931Rng mDrbg;

        internal X931SecureRandom(SecureRandom randomSource, X931Rng drbg, bool predictionResistant)
            : base(null)
        {
            this.mRandomSource = randomSource;
            this.mDrbg = drbg;
            this.mPredictionResistant = predictionResistant;
        }

        public override void SetSeed(byte[] seed)
        {
            lock (this)
            {
                if (mRandomSource != null)
                {
                    this.mRandomSource.SetSeed(seed);
                }
            }
        }

        public override void SetSeed(Span<byte> seed)
        {
            lock (this)
            {
                if (mRandomSource != null)
                {
                    this.mRandomSource.SetSeed(seed);
                }
            }
        }

        public override void SetSeed(long seed)
        {
            lock (this)
            {
                // this will happen when SecureRandom() is created
                if (mRandomSource != null)
                {
                    this.mRandomSource.SetSeed(seed);
                }
            }
        }

        public override void NextBytes(byte[] bytes)
        {
            NextBytes(bytes, 0, bytes.Length);
        }

        public override void NextBytes(byte[] buf, int off, int len)
        {
            NextBytes(buf.AsSpan(off, len));
        }

        public override void NextBytes(Span<byte> buffer)
        {
            lock (this)
            {
                // check if a reseed is required...
                if (mDrbg.Generate(buffer, mPredictionResistant) < 0)
                {
                    mDrbg.Reseed();
                    mDrbg.Generate(buffer, mPredictionResistant);
                }
            }
        }

        public override byte[] GenerateSeed(int numBytes)
        {
            return EntropyUtilities.GenerateSeed(mDrbg.EntropySource, numBytes);
        }

        public override void GenerateSeed(Span<byte> seed)
        {
            EntropyUtilities.GenerateSeed(mDrbg.EntropySource, seed);
        }
    }
}
