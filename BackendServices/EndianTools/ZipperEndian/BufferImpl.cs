using CustomLogger;
using System;

namespace EndianTools.ZipperEndian
{
    public static class BufferImpl
    {
        const int sizeOfChar = sizeof(byte);
        const int sizeOfShort = sizeof(short);
        const int sizeOfInt = sizeof(int);
        const int sizeOfULong = sizeof(ulong);

        public static bool ReadPrimitive(byte[] buffer, ref byte value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 8 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfChar : sizeOfChar + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToUInt8(temp, Endianness.Automatic, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 8;
            return true;
        }

        public static bool ReadPrimitive(byte[] buffer, ref sbyte value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 8 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfChar : sizeOfChar + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToInt8(temp, Endianness.Automatic, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 8;
            return true;
        }

        public static bool ReadPrimitive(byte[] buffer, ref short value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 16 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfShort : sizeOfShort + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToInt16(temp, Endianness.LittleEndian, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 16;
            return true;
        }

        public static bool ReadPrimitive(byte[] buffer, ref ushort value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 16 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfShort : sizeOfShort + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToUInt16(temp, Endianness.LittleEndian, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 16;
            return true;
        }

        public static bool ReadPrimitive(byte[] buffer, ref int value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 32 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfInt : sizeOfInt + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToInt32(temp, Endianness.LittleEndian, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 32;
            return true;
        }

        public static bool ReadPrimitive(byte[] buffer, ref uint value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 32 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfInt : sizeOfInt + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToUInt32(temp, Endianness.LittleEndian, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 32;
            return true;
        }

        public static bool ReadPrimitive(byte[] buffer, ref ulong value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 64 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfULong : sizeOfULong + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToUInt64(temp, Endianness.LittleEndian, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 64;
            return true;
        }

        public static bool ReadPrimitive(byte[] buffer, ref float value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;

            if (bitIndex + 32 > totalBits)
                return false; // not enough bits available

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfInt : sizeOfInt + 1;

            byte[] temp = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                temp[i] = (byte)(byteOffset + i < buffer.Length ? buffer[byteOffset + i] : 0);

            RawShift(temp, 0, byteCount, -bitOffset);

            value = EndianAwareConverter.ToSingle(temp, Endianness.LittleEndian, 0);

            if (rawDebug)
                LoggerAccessor.LogInfo($"[ReadPrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp)}");

            bitIndex += 32;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, byte value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 8 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfChar : sizeOfChar + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteUInt8(temp, Endianness.Automatic, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 8;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, sbyte value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 8 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfChar : sizeOfChar + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteInt8(temp, Endianness.Automatic, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 8;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, short value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 16 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfShort : sizeOfShort + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteInt16(temp, Endianness.BigEndian, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 16;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, ushort value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 16 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfShort : sizeOfShort + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteUInt16(temp, Endianness.BigEndian, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 16;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, int value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 32 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfInt : sizeOfInt + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteInt32(temp, Endianness.BigEndian, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 32;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, uint value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 32 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfInt : sizeOfInt + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteUInt32(temp, Endianness.BigEndian, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 32;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, ulong value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 64 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfULong : sizeOfULong + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteUInt64(temp, Endianness.BigEndian, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 64;
            return true;
        }

        public static bool WritePrimitive(byte[] buffer, float value, ref int bitIndex, bool rawDebug)
        {
            int totalBits = buffer.Length * 8;
            if (bitIndex + 32 > totalBits)
                return false; // not enough space

            int byteOffset = bitIndex / 8;
            int bitOffset = bitIndex % 8;
            int byteCount = bitOffset == 0 ? sizeOfInt : sizeOfInt + 1;

            byte[] temp = new byte[byteCount];
            EndianAwareConverter.WriteSingle(temp, Endianness.BigEndian, 0, value);

            // Shift left to align to bitOffset
            if (bitOffset != 0)
                RawShift(temp, 0, byteCount, bitOffset);

            // OR-write the bytes into buffer
            for (int i = 0; i < byteCount; i++)
            {
                int bufferIndex = byteOffset + i;
                if (bufferIndex >= buffer.Length)
                    break;
                buffer[bufferIndex] |= temp[i];
            }

            if (rawDebug)
                LoggerAccessor.LogInfo($"[WritePrimitive] value: {value}, bitIndex: {bitIndex}, bytes: {BitConverter.ToString(temp, 0, byteCount)}");

            bitIndex += 32;
            return true;
        }

        private static void RawShift(byte[] buffer, int start, int end, int amount)
        {
            if (start >= end || amount == 0)
                return;

            int length = end - start;

            if (amount < 0)
            {
                // Left shift
                int absAmount = -amount;
                int bitShift = absAmount & 7;
                int byteShift = absAmount / 8;

                // Move memory left by whole bytes
                if (byteShift > 0)
                {
                    Buffer.BlockCopy(buffer, start + byteShift, buffer, start, length - byteShift);
                    Array.Clear(buffer, end - byteShift, byteShift);
                }

                // Bit shift
                if (bitShift != 0)
                {
                    for (int i = start; i < end - 1; i++)
                    {
                        buffer[i] = (byte)((buffer[i] << bitShift) | (buffer[i + 1] >> (8 - bitShift)));
                    }
                    buffer[end - 1] <<= bitShift;
                }
            }
            else
            {
                // Right shift
                int bitShift = amount & 7;
                int byteShift = amount / 8;

                // Move memory right by whole bytes
                if (byteShift > 0)
                {
                    Buffer.BlockCopy(buffer, start, buffer, start + byteShift, length - byteShift);
                    Array.Clear(buffer, start, byteShift);
                }

                // Bit shift
                if (bitShift != 0)
                {
                    for (int i = end - 1; i > start; i--)
                    {
                        buffer[i] = (byte)((buffer[i] >> bitShift) | (buffer[i - 1] << (8 - bitShift)));
                    }
                    buffer[start] >>= bitShift;
                }
            }
        }
    }
}
