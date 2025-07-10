using System;
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif

namespace EndianTools
{
    public static class EndianAwareConverter
    {
        private static readonly bool isLittleEndianSystem = BitConverter.IsLittleEndian;

        public static ushort ToUInt16(byte[] buf, Endianness endianness, uint address)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            ReadOnlySpan<byte> span = buf.AsSpan((int)address, 2);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt16LittleEndian(span)
                : BinaryPrimitives.ReadUInt16BigEndian(span);
#else
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToUInt16(buf, (int)address);
            return BitConverter.ToUInt16(new byte[2]
            {
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
#endif
        }

        public static uint ToUInt32(byte[] buf, Endianness endianness, uint address)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            ReadOnlySpan<byte> span = buf.AsSpan((int)address, 4);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt32LittleEndian(span)
                : BinaryPrimitives.ReadUInt32BigEndian(span);
#else
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToUInt32(buf, (int)address);
            return BitConverter.ToUInt32(new byte[4]
            {
                buf[(int)address + 3],
                buf[(int)address + 2],
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
#endif
        }

        public static uint[] ToUInt32(byte[] buf, Endianness endianness)
        {
            // Note chars must be within ISO-8859-1 (with Unicode code-point < 256) to fit 4/uint
            uint[] l = new uint[(int)Math.Ceiling((double)buf.Length / 4)];

            // Create an array of uint, each holding the data of 4 characters
            // If the last block is less than 4 characters in length, fill with ascii null values
            for (int i = 0; i < l.Length; i++)
            {
                byte b0 = (i * 4) < buf.Length ? buf[i * 4] : (byte)0;
                byte b1 = (i * 4 + 1) < buf.Length ? buf[i * 4 + 1] : (byte)0;
                byte b2 = (i * 4 + 2) < buf.Length ? buf[i * 4 + 2] : (byte)0;
                byte b3 = (i * 4 + 3) < buf.Length ? buf[i * 4 + 3] : (byte)0;

                if (endianness == Endianness.LittleEndian)
                    // Little-endian: Least significant byte first
                    l[i] = (uint)(b0 | (b1 << 8) | (b2 << 16) | (b3 << 24));
                else
                    // Big-endian: Most significant byte first
                    l[i] = (uint)(b3 | (b2 << 8) | (b1 << 16) | (b0 << 24));
            }

            return l;
        }

        public static ulong ToUInt64(byte[] buf, Endianness endianness, uint address)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            ReadOnlySpan<byte> span = buf.AsSpan((int)address, 8);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt64LittleEndian(span)
                : BinaryPrimitives.ReadUInt64BigEndian(span);
#else
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToUInt64(buf, (int)address);
            return BitConverter.ToUInt64(new byte[8]
            {
                buf[(int)address + 7],
                buf[(int)address + 6],
                buf[(int)address + 5],
                buf[(int)address + 4],
                buf[(int)address + 3],
                buf[(int)address + 2],
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
#endif
        }

        public static short ToInt16(byte[] buf, Endianness endianness, uint address)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            ReadOnlySpan<byte> span = buf.AsSpan((int)address, 2);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt16LittleEndian(span)
                : BinaryPrimitives.ReadInt16BigEndian(span);
#else
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToInt16(buf, (int)address);
            return BitConverter.ToInt16(new byte[2]
            {
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
#endif
        }

        public static int ToInt32(byte[] buf, Endianness endianness, uint address)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            ReadOnlySpan<byte> span = buf.AsSpan((int)address, 4);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(span)
                : BinaryPrimitives.ReadInt32BigEndian(span);
#else
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToInt32(buf, (int)address);
            return BitConverter.ToInt32(new byte[4]
            {
                buf[(int)address + 3],
                buf[(int)address + 2],
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
#endif
        }

        public static long ToInt64(byte[] buf, Endianness endianness, uint address)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            ReadOnlySpan<byte> span = buf.AsSpan((int)address, 8);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt64LittleEndian(span)
                : BinaryPrimitives.ReadInt64BigEndian(span);
#else
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToInt64(buf, (int)address);
            return BitConverter.ToInt64(new byte[8]
            {
                buf[(int)address + 7],
                buf[(int)address + 6],
                buf[(int)address + 5],
                buf[(int)address + 4],
                buf[(int)address + 3],
                buf[(int)address + 2],
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
#endif
        }

        public static float ToSingle(byte[] buf, Endianness endianness, uint address)
        {
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToSingle(buf, (int)address);
            return BitConverter.ToSingle(new byte[4]
            {
                buf[(int)address + 3],
                buf[(int)address + 2],
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
        }

        public static double ToDouble(byte[] buf, Endianness endianness, uint address)
        {
            if (endianness == (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                return BitConverter.ToDouble(buf, (int)address);
            return BitConverter.ToDouble(new byte[8]
            {
                buf[(int)address + 7],
                buf[(int)address + 6],
                buf[(int)address + 5],
                buf[(int)address + 4],
                buf[(int)address + 3],
                buf[(int)address + 2],
                buf[(int)address + 1],
                buf[(int)address]
            }, 0);
        }

        public static void WriteInt16(byte[] buf, Endianness endianness, uint address, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 2);
        }

        public static void WriteUInt16(byte[] buf, Endianness endianness, uint address, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 2);
        }

        public static void WriteInt32(byte[] buf, Endianness endianness, uint address, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 4);
        }

        public static void WriteUInt32(byte[] buf, Endianness endianness, uint address, uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 4);
        }

        public static void WriteInt64(byte[] buf, Endianness endianness, uint address, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 8);
        }

        public static void WriteUInt64(byte[] buf, Endianness endianness, uint address, ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 8);
        }

        public static void WriteSingle(byte[] buf, Endianness endianness, uint address, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 4);
        }

        public static void WriteDouble(byte[] buf, Endianness endianness, uint address, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            Array.Copy(bytes, 0, buf, address, 8);
        }

        /// <summary>
        /// Convert array of longs back to utf-8 byte array
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBytes(uint[] buf, Endianness endianness)
        {
            const byte mask = byte.MaxValue;
            byte[] bytes = new byte[buf.Length * 4];

            // Split each long value into 4 separate characters (bytes) using the same format as ToLongs()
            for (int i = 0; i < buf.Length; i++)
            {
                uint value = buf[i];

                if (endianness == Endianness.LittleEndian)
                {
                    bytes[i * 4] = (byte)(value & mask);
                    bytes[i * 4 + 1] = (byte)((value >> 8) & mask);
                    bytes[i * 4 + 2] = (byte)((value >> 16) & mask);
                    bytes[i * 4 + 3] = (byte)((value >> 24) & mask);
                }
                else
                {
                    bytes[i * 4] = (byte)((value >> 24) & mask);
                    bytes[i * 4 + 1] = (byte)((value >> 16) & mask);
                    bytes[i * 4 + 2] = (byte)((value >> 8) & mask);
                    bytes[i * 4 + 3] = (byte)(value & mask);
                }
            }
            return bytes;
        }
    }
}
