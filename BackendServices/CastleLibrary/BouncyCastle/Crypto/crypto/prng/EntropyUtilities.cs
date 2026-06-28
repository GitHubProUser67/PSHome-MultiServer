using System;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Prng
{
    public abstract class EntropyUtilities
    {
        /**
         * Generate numBytes worth of entropy from the passed in entropy source.
         *
         * @param entropySource the entropy source to request the data from.
         * @param numBytes the number of bytes of entropy requested.
         * @return a byte array populated with the random data.
         */
        public static byte[] GenerateSeed(IEntropySource entropySource, int numBytes)
        {
            byte[] bytes = new byte[numBytes];

            GenerateSeed(entropySource, bytes);

            return bytes;
        }

        public static void GenerateSeed(IEntropySource entropySource, Span<byte> seed)
        {
            while (!seed.IsEmpty)
            {
                int len = entropySource.GetEntropy(seed);
                seed = seed[len..];
            }
        }
    }
}
