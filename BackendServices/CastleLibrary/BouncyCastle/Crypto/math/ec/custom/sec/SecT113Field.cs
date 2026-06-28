using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal static class SecT113Field
    {
        private const ulong M49 = ulong.MaxValue >> 15;
        private const ulong M57 = ulong.MaxValue >> 7;

        public static void Add(ReadOnlySpan<ulong> x, ReadOnlySpan<ulong> y, Span<ulong> z)
        {
            z[0] = x[0] ^ y[0];
            z[1] = x[1] ^ y[1];
        }

        public static void AddBothTo(ReadOnlySpan<ulong> x, ReadOnlySpan<ulong> y, Span<ulong> z)
        {
            z[0] ^= x[0] ^ y[0];
            z[1] ^= x[1] ^ y[1];
        }

        public static void AddExt(ReadOnlySpan<ulong> xx, ReadOnlySpan<ulong> yy, Span<ulong> zz)
        {
            zz[0] = xx[0] ^ yy[0];
            zz[1] = xx[1] ^ yy[1];
            zz[2] = xx[2] ^ yy[2];
            zz[3] = xx[3] ^ yy[3];
        }

        public static void AddOne(ReadOnlySpan<ulong> x, Span<ulong> z)
        {
            z[0] = x[0] ^ 1UL;
            z[1] = x[1];
        }

        public static void AddTo(ReadOnlySpan<ulong> x, Span<ulong> z)
        {
            z[0] ^= x[0];
            z[1] ^= x[1];
        }

        public static ulong[] FromBigInteger(BigInteger x)
        {
            return Nat.FromBigInteger64(113, x);
        }

        public static void HalfTrace(ReadOnlySpan<ulong> x, Span<ulong> z)
        {
            Span<ulong> tt = stackalloc ulong[4];

            Nat128.Copy64(x, z);
            for (int i = 1; i < 113; i += 2)
            {
                ImplSquare(z, tt);
                Reduce(tt, z);
                ImplSquare(z, tt);
                Reduce(tt, z);
                AddTo(x, z);
            }
        }

        public static void Invert(ReadOnlySpan<ulong> x, Span<ulong> z)
        {
            if (Nat128.IsZero64(x))
                throw new InvalidOperationException();

            // Itoh-Tsujii inversion

            Span<ulong> t0 = stackalloc ulong[2];
            Span<ulong> t1 = stackalloc ulong[2];

            Square(x, t0);
            Multiply(t0, x, t0);
            Square(t0, t0);
            Multiply(t0, x, t0);
            SquareN(t0, 3, t1);
            Multiply(t1, t0, t1);
            Square(t1, t1);
            Multiply(t1, x, t1);
            SquareN(t1, 7, t0);
            Multiply(t0, t1, t0);
            SquareN(t0, 14, t1);
            Multiply(t1, t0, t1);
            SquareN(t1, 28, t0);
            Multiply(t0, t1, t0);
            SquareN(t0, 56, t1);
            Multiply(t1, t0, t1);
            Square(t1, z);
        }

        public static void Multiply(ReadOnlySpan<ulong> x, ReadOnlySpan<ulong> y, Span<ulong> z)
        {
            Span<ulong> tt = stackalloc ulong[8];
            ImplMultiply(x, y, tt);
            Reduce(tt, z);
        }

        public static void MultiplyAddToExt(
            ReadOnlySpan<ulong> x,
            ReadOnlySpan<ulong> y,
            Span<ulong> zz
        )
        {
            Span<ulong> tt = stackalloc ulong[8];
            ImplMultiply(x, y, tt);
            AddExt(zz, tt, zz);
        }

        public static void MultiplyExt(ReadOnlySpan<ulong> x, ReadOnlySpan<ulong> y, Span<ulong> zz)
        {
            ImplMultiply(x, y, zz);
        }

        public static void Reduce(ReadOnlySpan<ulong> xx, Span<ulong> z)
        {
            ulong x0 = xx[0],
                x1 = xx[1],
                x2 = xx[2],
                x3 = xx[3];

            x1 ^= (x3 << 15) ^ (x3 << 24);
            x2 ^= (x3 >> 49) ^ (x3 >> 40);

            x0 ^= (x2 << 15) ^ (x2 << 24);
            x1 ^= (x2 >> 49) ^ (x2 >> 40);

            ulong t = x1 >> 49;
            z[0] = x0 ^ t ^ (t << 9);
            z[1] = x1 & M49;
        }

        public static void Reduce15(ulong[] z, int zOff)
        {
            ulong z1 = z[zOff + 1],
                t = z1 >> 49;
            z[zOff] ^= t ^ (t << 9);
            z[zOff + 1] = z1 & M49;
        }

        public static void Sqrt(ReadOnlySpan<ulong> x, Span<ulong> z)
        {
            ulong c0 = Interleave.Unshuffle(x[0], x[1], out ulong e0);

            z[0] = e0 ^ (c0 << 57) ^ (c0 << 5);
            z[1] = (c0 >> 7) ^ (c0 >> 59);
        }

        public static void Square(ReadOnlySpan<ulong> x, Span<ulong> z)
        {
            Span<ulong> tt = stackalloc ulong[4];
            ImplSquare(x, tt);
            Reduce(tt, z);
        }

        public static void SquareAddToExt(ReadOnlySpan<ulong> x, Span<ulong> zz)
        {
            Span<ulong> tt = stackalloc ulong[4];
            ImplSquare(x, tt);
            AddExt(zz, tt, zz);
        }

        public static void SquareExt(ReadOnlySpan<ulong> x, Span<ulong> zz)
        {
            ImplSquare(x, zz);
        }

        public static void SquareN(ReadOnlySpan<ulong> x, int n, Span<ulong> z)
        {
            Debug.Assert(n > 0);

            Span<ulong> tt = stackalloc ulong[4];
            ImplSquare(x, tt);
            Reduce(tt, z);

            while (--n > 0)
            {
                ImplSquare(z, tt);
                Reduce(tt, z);
            }
        }

        public static uint Trace(ReadOnlySpan<ulong> x)
        {
            // Non-zero-trace bits: 0
            return (uint)(x[0]) & 1U;
        }

        private static void ImplMultiply(
            ReadOnlySpan<ulong> x,
            ReadOnlySpan<ulong> y,
            Span<ulong> zz
        )
        {
            if (
                Org.BouncyCastle.Runtime.Intrinsics.X86.Pclmulqdq.IsEnabled
                && Org.BouncyCastle.Runtime.Intrinsics.Vector.IsPackedLittleEndian
            )
            {
                var X01 = Vector128.Create(x[0], x[1]);
                var Y01 = Vector128.Create(y[0], y[1]);

                var Z01 = Pclmulqdq.CarrylessMultiply(X01, Y01, 0x00);
                var Z12 = Sse2.Xor(
                    Pclmulqdq.CarrylessMultiply(X01, Y01, 0x01),
                    Pclmulqdq.CarrylessMultiply(X01, Y01, 0x10)
                );
                var Z23 = Pclmulqdq.CarrylessMultiply(X01, Y01, 0x11);

                Z01 = Sse2.Xor(Z01, Sse2.ShiftLeftLogical128BitLane(Z12, 8));
                Z23 = Sse2.Xor(Z23, Sse2.ShiftRightLogical128BitLane(Z12, 8));

                Span<byte> zzBytes = MemoryMarshal.AsBytes(zz);
                MemoryMarshal.Write(zzBytes[0x00..0x10], in Z01);
                MemoryMarshal.Write(zzBytes[0x10..0x20], in Z23);
                return;
            }

            /*
             * "Three-way recursion" as described in "Batch binary Edwards", Daniel J. Bernstein.
             */

            ulong f0 = x[0],
                f1 = x[1];
            f1 = ((f0 >> 57) ^ (f1 << 7)) & M57;
            f0 &= M57;

            ulong g0 = y[0],
                g1 = y[1];
            g1 = ((g0 >> 57) ^ (g1 << 7)) & M57;
            g0 &= M57;

            Span<ulong> u = zz;
            Span<ulong> H = stackalloc ulong[6];

            ImplMulw(u, f0, g0, H[0..]); // H(0)       57/56 bits
            ImplMulw(u, f1, g1, H[2..]); // H(INF)     57/54 bits
            ImplMulw(u, f0 ^ f1, g0 ^ g1, H[4..]); // H(1)       57/56 bits

            ulong r = H[1] ^ H[2];
            ulong z0 = H[0],
                z3 = H[3],
                z1 = H[4] ^ z0 ^ r,
                z2 = H[5] ^ z3 ^ r;

            zz[0] = z0 ^ (z1 << 57);
            zz[1] = (z1 >> 7) ^ (z2 << 50);
            zz[2] = (z2 >> 14) ^ (z3 << 43);
            zz[3] = (z3 >> 21);
        }

        private static void ImplMulw(Span<ulong> u, ulong x, ulong y, Span<ulong> z)
        {
            Debug.Assert(x >> 57 == 0);
            Debug.Assert(y >> 57 == 0);

            //u[0] = 0;
            u[1] = y;
            u[2] = u[1] << 1;
            u[3] = u[2] ^ y;
            u[4] = u[2] << 1;
            u[5] = u[4] ^ y;
            u[6] = u[3] << 1;
            u[7] = u[6] ^ y;

            uint j = (uint)x;
            ulong g,
                h = 0,
                l = u[(int)j & 7];
            int k = 48;
            do
            {
                j = (uint)(x >> k);
                g = u[(int)j & 7] ^ u[(int)(j >> 3) & 7] << 3 ^ u[(int)(j >> 6) & 7] << 6;
                l ^= (g << k);
                h ^= (g >> -k);
            } while ((k -= 9) > 0);

            h ^= ((x & 0x0100804020100800UL) & (ulong)(((long)y << 7) >> 63)) >> 8;

            Debug.Assert(h >> 49 == 0);

            z[0] ^= l & M57;
            z[1] ^= (l >> 57) ^ (h << 7);
        }

        private static void ImplSquare(ReadOnlySpan<ulong> x, Span<ulong> zz)
        {
            if (Org.BouncyCastle.Runtime.Intrinsics.X86.Bmi2.X64.IsEnabled)
            {
                zz[3] = Bmi2.X64.ParallelBitDeposit(x[1] >> 32, 0x5555555555555555UL);
                zz[2] = Bmi2.X64.ParallelBitDeposit(x[1], 0x5555555555555555UL);
                zz[1] = Bmi2.X64.ParallelBitDeposit(x[0] >> 32, 0x5555555555555555UL);
                zz[0] = Bmi2.X64.ParallelBitDeposit(x[0], 0x5555555555555555UL);
                return;
            }

            Interleave.Expand64To128(x[..2], zz[..4]);
        }
    }
}
