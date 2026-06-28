using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Kems.MLKem
{
    internal class MLKemEngine
    {
        private readonly IndCpa m_indCpa;
        private readonly SecureRandom m_random;

        // Constant Parameters
        internal const int N = 256;
        internal const int Q = 3329;
        internal const int QInv = 62209;

        internal const int SymBytes = 32;
        private const int SharedSecretBytes = 32;

        internal const int PolyBytes = 384;

        internal const int Eta2 = 2;

        internal int IndCpaMsgBytes = SymBytes;
        internal Symmetric Symmetric { get; private set; }

        // Parameters
        internal int K { get; private set; }
        internal int PolyVecBytes { get; private set; }
        internal int PolyCompressedBytes { get; private set; }
        internal int PolyVecCompressedBytes { get; private set; }
        internal int Eta1 { get; private set; }
        internal int IndCpaPublicKeyBytes { get; private set; }
        internal int IndCpaSecretKeyBytes { get; private set; }
        internal int IndCpaBytes { get; private set; }
        internal int PublicKeyBytes { get; private set; }
        internal int SecretKeyBytes { get; private set; }
        internal int CipherTextBytes { get; private set; }

        // Crypto
        internal int CryptoBytes { get; private set; }
        internal int CryptoSecretKeyBytes { get; private set; }
        internal int CryptoPublicKeyBytes { get; private set; }
        internal int CryptoCipherTextBytes { get; private set; }

        internal MLKemEngine(int k, SecureRandom random)
        {
            K = k;
            switch (k)
            {
                case 2:
                    Eta1 = 3;
                    PolyCompressedBytes = 128;
                    PolyVecCompressedBytes = K * 320;
                    break;
                case 3:
                    Eta1 = 2;
                    PolyCompressedBytes = 128;
                    PolyVecCompressedBytes = K * 320;
                    break;
                case 4:
                    Eta1 = 2;
                    PolyCompressedBytes = 160;
                    PolyVecCompressedBytes = K * 352;
                    break;
            }

            PolyVecBytes = k * PolyBytes;
            IndCpaPublicKeyBytes = PolyVecBytes + SymBytes;
            IndCpaSecretKeyBytes = PolyVecBytes;
            IndCpaBytes = PolyVecCompressedBytes + PolyCompressedBytes;
            PublicKeyBytes = IndCpaPublicKeyBytes;
            SecretKeyBytes = IndCpaSecretKeyBytes + IndCpaPublicKeyBytes + 2 * SymBytes;
            CipherTextBytes = IndCpaBytes;

            // Define Crypto Params
            CryptoBytes = SharedSecretBytes;
            CryptoSecretKeyBytes = SecretKeyBytes;
            CryptoPublicKeyBytes = PublicKeyBytes;
            CryptoCipherTextBytes = CipherTextBytes;
            Symmetric = new Symmetric.ShakeSymmetric();

            m_indCpa = new IndCpa(this);
            m_random = random;
        }

        internal SecureRandom Random => m_random;

        internal bool CheckModulus(byte[] t) => PolyVec.CheckModulus(this, t) < 0;

        internal void GenerateKemKeyPair(
            out byte[] t,
            out byte[] rho,
            out byte[] s,
            out byte[] hpk,
            out byte[] nonce,
            out byte[] seed
        )
        {
            byte[] d = new byte[SymBytes];
            byte[] z = new byte[SymBytes];
            m_random.NextBytes(d);
            m_random.NextBytes(z);

            GenerateKemKeyPairInternal(d, z, out t, out rho, out s, out hpk, out nonce, out seed);
        }

        internal void GenerateKemKeyPairInternal(
            byte[] d,
            byte[] z,
            out byte[] t,
            out byte[] rho,
            out byte[] s,
            out byte[] hpk,
            out byte[] nonce,
            out byte[] seed
        )
        {
            m_indCpa.GenerateKeyPair(d, out byte[] pk, out s);
            Debug.Assert(s.Length == IndCpaSecretKeyBytes);

            hpk = new byte[32];
            Symmetric.Hash_h(pk.AsSpan(), hpk.AsSpan());

            t = Arrays.CopyOfRange(pk, 0, IndCpaPublicKeyBytes - 32);
            rho = Arrays.CopyOfRange(pk, IndCpaPublicKeyBytes - 32, IndCpaPublicKeyBytes);
            nonce = z;
            seed = Arrays.Concatenate(d, z);
        }

        internal void KemDecrypt(
            Span<byte> secret,
            ReadOnlySpan<byte> encapsulation,
            MLKemPrivateKeyParameters privateKey
        )
        {
            byte[] secretKey = privateKey.GetEncoded();

            // TODO do input validation
            Span<byte> kr = stackalloc byte[2 * SymBytes];
            Span<byte> buf = stackalloc byte[2 * SymBytes];
            Span<byte> cmp = stackalloc byte[CipherTextBytes];
            ReadOnlySpan<byte> pk = secretKey.AsSpan(IndCpaSecretKeyBytes);
            Span<byte> implicit_rejection = stackalloc byte[SymBytes + CipherTextBytes];

            m_indCpa.Decrypt(buf, encapsulation, secretKey);
            secretKey.AsSpan(SecretKeyBytes - 2 * SymBytes, SymBytes).CopyTo(buf[SymBytes..]);

            Symmetric.Hash_g(buf, kr);

            m_indCpa.Encrypt(cmp, buf[..SymBytes], pk, kr[SymBytes..]);

            int fail = ~FixedTimeEquals(cmp, encapsulation);

            Symmetric.Hash_h(encapsulation, kr[SymBytes..]);
            secretKey.AsSpan(SecretKeyBytes - SymBytes, SymBytes).CopyTo(implicit_rejection);
            encapsulation.CopyTo(implicit_rejection[SymBytes..]);
            Symmetric.Kdf(implicit_rejection, implicit_rejection);

            CMov(kr, implicit_rejection, SymBytes, fail);

            kr[..SharedSecretBytes].CopyTo(secret);
        }

        internal void KemEncrypt(
            Span<byte> encapsulation,
            Span<byte> secret,
            MLKemPublicKeyParameters publicKey,
            ReadOnlySpan<byte> randBytes
        )
        {
            ReadOnlySpan<byte> pk = publicKey.GetEncoded();

            Span<byte> buf = stackalloc byte[2 * SymBytes];
            Span<byte> kr = stackalloc byte[2 * SymBytes];

            randBytes[..SymBytes].CopyTo(buf);

            Symmetric.Hash_h(pk, buf[SymBytes..]);

            Symmetric.Hash_g(buf, kr);

            m_indCpa.Encrypt(encapsulation, buf[..SymBytes], pk, kr[SymBytes..]);

            kr[..SharedSecretBytes].CopyTo(secret);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void CMov(Span<byte> r, ReadOnlySpan<byte> x, int xLen, int cond)
        {
            Debug.Assert(0 == cond || -1 == cond);

            for (int i = 0; i < xLen; ++i)
            {
                int r_i = r[i],
                    diff = r_i ^ x[i];
                r_i ^= diff & cond;
                r[i] = (byte)r_i;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static int FixedTimeEquals(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            int d = 0;
            for (int i = 0, len = a.Length; i < len; ++i)
            {
                d |= a[i] ^ b[i];
            }
            d |= d >> 16;
            d &= 0xFFFF;
            return (d - 1) >> 31;
        }
    }
}
