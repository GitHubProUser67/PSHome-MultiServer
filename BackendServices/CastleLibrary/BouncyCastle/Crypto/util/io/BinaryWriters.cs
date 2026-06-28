using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
    public static class BinaryWriters
    {
        public static void WriteInt16BigEndian(BinaryWriter binaryWriter, short n)
        {
            short bigEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? Shorts.ReverseBytes(n)
                : n;
            binaryWriter.Write(bigEndian);
        }

        public static void WriteInt16LittleEndian(BinaryWriter binaryWriter, short n)
        {
            short littleEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? n
                : Shorts.ReverseBytes(n);
            binaryWriter.Write(littleEndian);
        }

        public static void WriteInt32BigEndian(BinaryWriter binaryWriter, int n)
        {
            int bigEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? Integers.ReverseBytes(n)
                : n;
            binaryWriter.Write(bigEndian);
        }

        public static void WriteInt32LittleEndian(BinaryWriter binaryWriter, int n)
        {
            int littleEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? n
                : Integers.ReverseBytes(n);
            binaryWriter.Write(littleEndian);
        }

        public static void WriteInt64BigEndian(BinaryWriter binaryWriter, long n)
        {
            long bigEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? Longs.ReverseBytes(n)
                : n;
            binaryWriter.Write(bigEndian);
        }

        public static void WriteInt64LittleEndian(BinaryWriter binaryWriter, long n)
        {
            long littleEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? n
                : Longs.ReverseBytes(n);
            binaryWriter.Write(littleEndian);
        }

        [CLSCompliant(false)]
        public static void WriteUInt16BigEndian(BinaryWriter binaryWriter, ushort n)
        {
            ushort bigEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? Shorts.ReverseBytes(n)
                : n;
            binaryWriter.Write(bigEndian);
        }

        [CLSCompliant(false)]
        public static void WriteUInt16LittleEndian(BinaryWriter binaryWriter, ushort n)
        {
            ushort littleEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? n
                : Shorts.ReverseBytes(n);
            binaryWriter.Write(littleEndian);
        }

        [CLSCompliant(false)]
        public static void WriteUInt32BigEndian(BinaryWriter binaryWriter, uint n)
        {
            uint bigEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? Integers.ReverseBytes(n)
                : n;
            binaryWriter.Write(bigEndian);
        }

        [CLSCompliant(false)]
        public static void WriteUInt32LittleEndian(BinaryWriter binaryWriter, uint n)
        {
            uint littleEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? n
                : Integers.ReverseBytes(n);
            binaryWriter.Write(littleEndian);
        }

        [CLSCompliant(false)]
        public static void WriteUInt64BigEndian(BinaryWriter binaryWriter, ulong n)
        {
            ulong bigEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? Longs.ReverseBytes(n)
                : n;
            binaryWriter.Write(bigEndian);
        }

        [CLSCompliant(false)]
        public static void WriteUInt64LittleEndian(BinaryWriter binaryWriter, ulong n)
        {
            ulong littleEndian = EndianTools.EndianAwareConverter.isLittleEndianSystem
                ? n
                : Longs.ReverseBytes(n);
            binaryWriter.Write(littleEndian);
        }
    }
}
