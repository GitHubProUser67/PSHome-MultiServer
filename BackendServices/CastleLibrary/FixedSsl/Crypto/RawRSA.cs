using System.Numerics;
using System.Security.Cryptography;

namespace CastleLibrary.FixedSsl.Crypto
{
    internal static class RawRSA
    {
        public static byte[] EncryptValue(RSA rsa, byte[] value)
        {
            RSAParameters p = rsa.ExportParameters(false);

            return ToFixed(
                BigInteger.ModPow(
                    new BigInteger(value, isUnsigned: true, isBigEndian: true),
                    new BigInteger(p.Exponent, isUnsigned: true, isBigEndian: true),
                    new BigInteger(p.Modulus, isUnsigned: true, isBigEndian: true)
                ),
                p.Modulus.Length
            );
        }

        public static byte[] DecryptValue(RSA rsa, byte[] value)
        {
            RSAParameters p = rsa.ExportParameters(true);

            BigInteger r,
                n = new(p.Modulus, isUnsigned: true, isBigEndian: true),
                input = new(value, isUnsigned: true, isBigEndian: true);

            using (var d = RandomNumberGenerator.Create())
            {
                byte[] e = new byte[p.Modulus.Length];
                do
                {
                    d.GetBytes(e);
                    r = new BigInteger(e, isUnsigned: true, isBigEndian: true);
                } while (r <= 1 || r >= n);
            }

            return ToFixed(
                (
                    BigInteger.ModPow(
                        (
                            BigInteger.ModPow(
                                r,
                                new BigInteger(p.Exponent, isUnsigned: true, isBigEndian: true),
                                n
                            ) * input
                        ) % n,
                        new BigInteger(p.D, isUnsigned: true, isBigEndian: true),
                        n
                    ) * ModInverse(r, n)
                ) % n,
                p.Modulus.Length
            );
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger n)
        {
            BigInteger t = 0,
                newT = 1;
            BigInteger r = n,
                newR = a;

            while (newR != 0)
            {
                BigInteger quotient = r / newR;

                (t, newT) = (newT, t - quotient * newT);
                (r, newR) = (newR, r - quotient * newR);
            }

            if (r > 1)
                throw new ArithmeticException("[RawRSA] - ModInverse: Not invertible");

            if (t < 0)
                t += n;

            return t;
        }

        private static byte[] ToFixed(BigInteger value, int length)
        {
            byte[] tmp = value.ToByteArray(isUnsigned: true, isBigEndian: true);

            if (tmp.Length == length)
                return tmp;

            byte[] result = new byte[length];
            Buffer.BlockCopy(tmp, 0, result, length - tmp.Length, tmp.Length);
            return result;
        }
    }
}
