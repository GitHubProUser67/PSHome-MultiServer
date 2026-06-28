using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Utilities
{
    public static class Longs
    {
        public const int NumBits = 64;
        public const int NumBytes = 8;

        public static int Compare(long x, long y) => x.CompareTo(y);

        [CLSCompliant(false)]
        public static int Compare(ulong x, ulong y) => x.CompareTo(y);

        public static int CompareUnsigned(long x, long y) => Compare((ulong)x, (ulong)y);

        public static long HighestOneBit(long i)
        {
            return (long)HighestOneBit((ulong)i);
        }

        [CLSCompliant(false)]
        public static ulong HighestOneBit(ulong i)
        {
            i |= i >> 1;
            i |= i >> 2;
            i |= i >> 4;
            i |= i >> 8;
            i |= i >> 16;
            i |= i >> 32;
            return i - (i >> 1);
        }

        public static long LowestOneBit(long i)
        {
            return i & -i;
        }

        [CLSCompliant(false)]
        public static ulong LowestOneBit(ulong i)
        {
            return (ulong)LowestOneBit((long)i);
        }

        public static int NumberOfLeadingZeros(long i)
        {
            return BitOperations.LeadingZeroCount((ulong)i);
        }

        public static int NumberOfTrailingZeros(long i)
        {
            return BitOperations.TrailingZeroCount((ulong)i);
        }

        public static int PopCount(long i)
        {
            return PopCount((ulong)i);
        }

        [CLSCompliant(false)]
        public static int PopCount(ulong u)
        {
            return BitOperations.PopCount(u);
        }

        public static long Reverse(long i)
        {
            return (long)Reverse((ulong)i);
        }

        [CLSCompliant(false)]
        public static ulong Reverse(ulong i)
        {
            i = Bits.BitPermuteStepSimple(i, 0x5555555555555555UL, 1);
            i = Bits.BitPermuteStepSimple(i, 0x3333333333333333UL, 2);
            i = Bits.BitPermuteStepSimple(i, 0x0F0F0F0F0F0F0F0FUL, 4);
            return ReverseBytes(i);
        }

        public static long ReverseBytes(long i)
        {
            return BinaryPrimitives.ReverseEndianness(i);
        }

        [CLSCompliant(false)]
        public static ulong ReverseBytes(ulong i)
        {
            return BinaryPrimitives.ReverseEndianness(i);
        }

        public static long RotateLeft(long i, int distance)
        {
            return (long)BitOperations.RotateLeft((ulong)i, distance);
        }

        [CLSCompliant(false)]
        public static ulong RotateLeft(ulong i, int distance)
        {
            return BitOperations.RotateLeft(i, distance);
        }

        public static long RotateRight(long i, int distance)
        {
            return (long)BitOperations.RotateRight((ulong)i, distance);
        }

        [CLSCompliant(false)]
        public static ulong RotateRight(ulong i, int distance)
        {
            return BitOperations.RotateRight(i, distance);
        }
    }
}
