using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using Org.BouncyCastle.Math.Raw;

namespace Org.BouncyCastle.Utilities
{
    public static class Integers
    {
        public const int NumBits = 32;
        public const int NumBytes = 4;

        public static int Compare(int x, int y) => x.CompareTo(y);

        [CLSCompliant(false)]
        public static int Compare(uint x, uint y) => x.CompareTo(y);

        public static int CompareUnsigned(int x, int y) => Compare((uint)x, (uint)y);

        public static int HighestOneBit(int i)
        {
            return (int)HighestOneBit((uint)i);
        }

        [CLSCompliant(false)]
        public static uint HighestOneBit(uint i)
        {
            i |= i >> 1;
            i |= i >> 2;
            i |= i >> 4;
            i |= i >> 8;
            i |= i >> 16;
            return i - (i >> 1);
        }

        public static int LowestOneBit(int i)
        {
            return i & -i;
        }

        [CLSCompliant(false)]
        public static uint LowestOneBit(uint i)
        {
            return (uint)LowestOneBit((int)i);
        }

        public static int NumberOfLeadingZeros(int i)
        {
            return BitOperations.LeadingZeroCount((uint)i);
        }

        public static int NumberOfTrailingZeros(int i)
        {
            return BitOperations.TrailingZeroCount(i);
        }

        public static int PopCount(int i)
        {
            return PopCount((uint)i);
        }

        [CLSCompliant(false)]
        public static int PopCount(uint u)
        {
            return BitOperations.PopCount(u);
        }

        public static int Reverse(int i)
        {
            return (int)Reverse((uint)i);
        }

        [CLSCompliant(false)]
        public static uint Reverse(uint i)
        {
            i = Bits.BitPermuteStepSimple(i, 0x55555555U, 1);
            i = Bits.BitPermuteStepSimple(i, 0x33333333U, 2);
            i = Bits.BitPermuteStepSimple(i, 0x0F0F0F0FU, 4);
            return ReverseBytes(i);
        }

        public static int ReverseBytes(int i)
        {
            return BinaryPrimitives.ReverseEndianness(i);
        }

        [CLSCompliant(false)]
        public static uint ReverseBytes(uint i)
        {
            return BinaryPrimitives.ReverseEndianness(i);
        }

        public static int RotateLeft(int i, int distance)
        {
            return (int)BitOperations.RotateLeft((uint)i, distance);
        }

        [CLSCompliant(false)]
        public static uint RotateLeft(uint i, int distance)
        {
            return BitOperations.RotateLeft(i, distance);
        }

        public static int RotateRight(int i, int distance)
        {
            return (int)BitOperations.RotateRight((uint)i, distance);
        }

        [CLSCompliant(false)]
        public static uint RotateRight(uint i, int distance)
        {
            return BitOperations.RotateRight(i, distance);
        }
    }
}
