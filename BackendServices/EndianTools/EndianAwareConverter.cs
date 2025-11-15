using System;
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Buffers.Binary;
#endif

namespace EndianTools
{
    // Mofified from the TDUWorld solution (massive thanks to them).
    public static class EndianAwareConverter
    {
        public static readonly bool isLittleEndianSystem = BitConverter.IsLittleEndian;
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public static byte ToUInt8(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            if (endianness != Endianness.Automatic)
                throw new ArgumentException("[EndianAwareConverter] - UInt8 reads doesn't have an endianness to resolve to");

            return buf[(int)address];
        }

        public static ushort ToUInt16(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            ReadOnlySpan<byte> span = buf.Slice((int)address, 2);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt16LittleEndian(span)
                : BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        public static int ToUInt24(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            int addressInt = (int)address;
            if (endianness == Endianness.LittleEndian)
                return buf[addressInt] | (buf[addressInt + 1] << 8) | (buf[addressInt + 2] << 16);
            return (buf[addressInt] << 16) | (buf[addressInt + 1] << 8) | buf[addressInt + 2];
        }

        public static uint ToUInt32(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            ReadOnlySpan<byte> span = buf.Slice((int)address, 4);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt32LittleEndian(span)
                : BinaryPrimitives.ReadUInt32BigEndian(span);
        }

        public static uint[] ToUInt32(ReadOnlySpan<byte> buf, Endianness endianness)
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

        public static ulong ToUInt64(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            ReadOnlySpan<byte> span = buf.Slice((int)address, 8);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadUInt64LittleEndian(span)
                : BinaryPrimitives.ReadUInt64BigEndian(span);
        }

        public static sbyte ToInt8(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            if (endianness != Endianness.Automatic)
                throw new ArgumentException("[EndianAwareConverter] - Int8 reads doesn't have an endianness to resolve to");

            return (sbyte)buf[(int)address];
        }

        public static short ToInt16(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            ReadOnlySpan<byte> span = buf.Slice((int)address, 2);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt16LittleEndian(span)
                : BinaryPrimitives.ReadInt16BigEndian(span);
        }

        public static int ToInt32(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            ReadOnlySpan<byte> span = buf.Slice((int)address, 4);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(span)
                : BinaryPrimitives.ReadInt32BigEndian(span);
        }

        public static long ToInt64(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            ReadOnlySpan<byte> span = buf.Slice((int)address, 8);
            return endianness == Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt64LittleEndian(span)
                : BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public static float ToSingle(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            int addressInt = (int)address;
            Span<byte> temp = stackalloc byte[4];
            if (endianness == Endianness.LittleEndian)
                buf.Slice(addressInt, 4).CopyTo(temp);
            else
            {
                temp[0] = buf[addressInt + 3];
                temp[1] = buf[addressInt + 2];
                temp[2] = buf[addressInt + 1];
                temp[3] = buf[addressInt];
            }
            return BitConverter.ToSingle(temp);
        }

        public static double ToDouble(ReadOnlySpan<byte> buf, Endianness endianness, uint address)
        {
            int addressInt = (int)address;
            Span<byte> temp = stackalloc byte[8];
            if (endianness == Endianness.LittleEndian)
                buf.Slice(addressInt, 8).CopyTo(temp);
            else
            {
                temp[0] = buf[addressInt + 7];
                temp[1] = buf[addressInt + 6];
                temp[2] = buf[addressInt + 5];
                temp[3] = buf[addressInt + 4];
                temp[4] = buf[addressInt + 3];
                temp[5] = buf[addressInt + 2];
                temp[6] = buf[addressInt + 1];
                temp[7] = buf[addressInt];
            }
            return BitConverter.ToDouble(temp);
        }
#endif
        public static byte ToUInt8(byte[] buf, Endianness endianness, uint address)
        {
            if (endianness != Endianness.Automatic)
                throw new ArgumentException("[EndianAwareConverter] - UInt8 reads doesn't have an endianness to resolve to");

            return buf[(int)address];
        }

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

        public static int ToUInt24(byte[] buf, Endianness endianness, uint address)
        {
            if (endianness == Endianness.LittleEndian)
                return buf[address] | (buf[address + 1] << 8) | (buf[address + 2] << 16);
            return (buf[address] << 16) | (buf[address + 1] << 8) | buf[address + 2];
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

        public static sbyte ToInt8(byte[] buf, Endianness endianness, uint address)
        {
            if (endianness != Endianness.Automatic)
                throw new ArgumentException("[EndianAwareConverter] - Int8 reads doesn't have an endianness to resolve to");

            return (sbyte)buf[(int)address];
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

        // Does not have the trimming system (beware of the input buffer)
        public static void WriteUInt8(byte[] buf, Endianness endianness, uint address, byte value)
        {
            if (endianness != Endianness.Automatic)
                throw new ArgumentException("[EndianAwareConverter] - UInt8 writes doesn't have an endianness to resolve to");

            byte[] bytes = new byte[] { value };
            CopyToBuffer(buf, bytes, address, false);
        }

        public static void WriteUInt16(byte[] buf, Endianness endianness, uint address, ushort value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        public static void WriteUInt32(byte[] buf, Endianness endianness, uint address, uint value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        public static void WriteUInt64(byte[] buf, Endianness endianness, uint address, ulong value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        public static void WriteInt8(byte[] buf, Endianness endianness, uint address, sbyte value)
        {
            if (endianness != Endianness.Automatic)
                throw new ArgumentException("[EndianAwareConverter] - Int8 writes doesn't have an endianness to resolve to");

            byte[] bytes = new byte[] { (byte)value };
            CopyToBuffer(buf, bytes, address, false);
        }


        public static void WriteInt16(byte[] buf, Endianness endianness, uint address, short value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        public static void WriteInt32(byte[] buf, Endianness endianness, uint address, int value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        public static void WriteInt64(byte[] buf, Endianness endianness, uint address, long value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        public static void WriteSingle(byte[] buf, Endianness endianness, uint address, float value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        public static void WriteDouble(byte[] buf, Endianness endianness, uint address, double value, bool trimAtStart = false)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (endianness != (isLittleEndianSystem ? Endianness.LittleEndian : Endianness.BigEndian))
                Array.Reverse(bytes);
            CopyToBuffer(buf, bytes, address, trimAtStart);
        }

        private static void CopyToBuffer(byte[] buf, byte[] bytes, uint address, bool trimAtStart)
        {
            long copyLength = Math.Min(bytes.Length, buf.LongLength - address);
            Array.Copy(bytes, trimAtStart ? bytes.Length - copyLength : 0, buf, address, copyLength);
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
                int address = i * 4;

                if (endianness == Endianness.LittleEndian)
                {
                    bytes[address] = (byte)(value & mask);
                    bytes[address + 1] = (byte)((value >> 8) & mask);
                    bytes[address + 2] = (byte)((value >> 16) & mask);
                    bytes[address + 3] = (byte)((value >> 24) & mask);
                }
                else
                {
                    bytes[address] = (byte)((value >> 24) & mask);
                    bytes[address + 1] = (byte)((value >> 16) & mask);
                    bytes[address + 2] = (byte)((value >> 8) & mask);
                    bytes[address + 3] = (byte)(value & mask);
                }
            }
            return bytes;
        }
    }
}
