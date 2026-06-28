using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Org.BouncyCastle.Utilities
{
    public static class Bytes
    {
        public const int NumBits = 8;
        public const int NumBytes = 1;

        public static void Xor(int len, byte[] x, byte[] y, byte[] z)
        {
            Xor(len, x.AsSpan(0, len), y.AsSpan(0, len), z.AsSpan(0, len));
        }

        public static void Xor(int len, byte[] x, int xOff, byte[] y, int yOff, byte[] z, int zOff)
        {
            Xor(len, x.AsSpan(xOff, len), y.AsSpan(yOff, len), z.AsSpan(zOff, len));
        }

        public static void Xor(int len, ReadOnlySpan<byte> x, ReadOnlySpan<byte> y, Span<byte> z)
        {
            int i = 0;
            if (Vector.IsHardwareAccelerated)
            {
                int limit = len - Vector<byte>.Count;
                while (i <= limit)
                {
                    var vx = new Vector<byte>(x[i..]);
                    var vy = new Vector<byte>(y[i..]);
                    (vx ^ vy).CopyTo(z[i..]);
                    i += Vector<byte>.Count;
                }
            }
            {
                int limit = len - 8;
                while (i <= limit)
                {
                    ulong x64 = MemoryMarshal.Read<ulong>(x[i..]);
                    ulong y64 = MemoryMarshal.Read<ulong>(y[i..]);
                    ulong z64 = x64 ^ y64;
                    MemoryMarshal.Write(z[i..], in z64);
                    i += 8;
                }
            }
            {
                while (i < len)
                {
                    z[i] = (byte)(x[i] ^ y[i]);
                    ++i;
                }
            }
        }

        public static void XorTo(int len, byte[] x, byte[] z)
        {
            XorTo(len, x.AsSpan(0, len), z.AsSpan(0, len));
        }

        public static void XorTo(int len, byte[] x, int xOff, byte[] z, int zOff)
        {
            XorTo(len, x.AsSpan(xOff, len), z.AsSpan(zOff, len));
        }

        public static void XorTo(int len, ReadOnlySpan<byte> x, Span<byte> z)
        {
            int i = 0;
            if (Vector.IsHardwareAccelerated)
            {
                int limit = len - Vector<byte>.Count;
                while (i <= limit)
                {
                    var vx = new Vector<byte>(x[i..]);
                    var vz = new Vector<byte>(z[i..]);
                    (vx ^ vz).CopyTo(z[i..]);
                    i += Vector<byte>.Count;
                }
            }
            {
                int limit = len - 8;
                while (i <= limit)
                {
                    ulong x64 = MemoryMarshal.Read<ulong>(x[i..]);
                    ulong z64 = MemoryMarshal.Read<ulong>(z[i..]);
                    z64 ^= x64;
                    MemoryMarshal.Write(z[i..], in z64);
                    i += 8;
                }
            }
            {
                while (i < len)
                {
                    z[i] ^= x[i];
                    ++i;
                }
            }
        }
    }
}
