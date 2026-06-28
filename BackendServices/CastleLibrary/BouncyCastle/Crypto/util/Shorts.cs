using System;
using System.Buffers.Binary;

namespace Org.BouncyCastle.Utilities
{
    public static class Shorts
    {
        public const int NumBits = 16;
        public const int NumBytes = 2;

        public static short ReverseBytes(short i)
        {
            return BinaryPrimitives.ReverseEndianness(i);
        }

        [CLSCompliant(false)]
        public static ushort ReverseBytes(ushort i)
        {
            return BinaryPrimitives.ReverseEndianness(i);
        }

        public static short RotateLeft(short i, int distance)
        {
            return (short)RotateLeft((ushort)i, distance);
        }

        [CLSCompliant(false)]
        public static ushort RotateLeft(ushort i, int distance)
        {
            return (ushort)((i << distance) | (i >> (16 - distance)));
        }

        public static short RotateRight(short i, int distance)
        {
            return (short)RotateRight((ushort)i, distance);
        }

        [CLSCompliant(false)]
        public static ushort RotateRight(ushort i, int distance)
        {
            return (ushort)((i >> distance) | (i << (16 - distance)));
        }
    }
}
