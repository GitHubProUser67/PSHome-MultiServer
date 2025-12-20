using System.Numerics;
using System.Text;

namespace Tdf.Extensions
{
    internal static class TdfStreamExtensions
    {
        internal static BigInteger? ReadTdfInteger(this Stream stream)
        {
            BigInteger b, value = stream.ReadByte();

            if (value == -1)
                return null;

            byte i = 6;

            bool negative = (value & 0x40) != 0;
            bool readNext = (value & 0x80) != 0;
            value &= 0x3F;

            while (readNext)
            {
                b = stream.ReadByte();

                if (b == -1)
                    return null;

                value |= (b & 0x7F) << i;
                i += 7;

                readNext = b >> 7 != 0;
            }

            if (negative)
                return value != 0 ? -value : long.MinValue;
            return value;
        }

        internal static BigInteger? ReadTdfLegacyInteger(this Stream stream, byte size)
        {
            if (size < 15)
                return size;

            return stream.ReadTdfLegacyInteger();
        }

        internal static BigInteger? ReadTdfLegacyInteger(this Stream stream)
        {
            BigInteger b, value = stream.ReadByte();
            if (value == -1)
                return null;

            bool readNext = (value & 0x80) != 0;
            value &= 0x7F;

            while (readNext)
            {
                b = stream.ReadByte();
                if (b == -1)
                    return null;

                value = (value << 7) | (b & 0x7F);
                readNext = b >> 7 != 0;
            }

            return value;
        }

        internal static string? ReadTdfString(this Stream stream)
        {
            byte[]? data = stream.ReadTdfBlob();
            if (data == null)
                return null;


            //checking whether we should include last char in the string or not
            int len = data.Length;
            if (len > 0)
            {
                int lengthWithoutTrailingByte = len - 1;
                if (data[lengthWithoutTrailingByte] == 0x00)
                    len = lengthWithoutTrailingByte;
            }

            return Encoding.UTF8.GetString(data, 0, len);
        }

        internal static string? ReadTdfLegacyString(this Stream stream, byte size)
        {
            byte[]? data = stream.ReadTdfLegacyBlob(size);
            if (data == null)
                return null;

            //checking whether we should include last char in the string or not
            int len = data.Length;
            if (len > 0)
            {
                int lengthWithoutTrailingByte = len - 1;
                if (data[lengthWithoutTrailingByte] == 0x00)
                    len = lengthWithoutTrailingByte;
            }

            return Encoding.UTF8.GetString(data, 0, len);
        }

        internal static byte[]? ReadTdfBlob(this Stream stream)
        {
            BigInteger? len = stream.ReadTdfInteger();
            if (len == null || len.Value < 0)
                return null;

            byte[] blob = new byte[(int)len.Value];

            if (!stream.ReadAll(blob, 0, blob.Length))
                return null;

            return blob;
        }

        internal static byte[]? ReadTdfLegacyBlob(this Stream stream, byte size)
        {
            BigInteger? len = stream.ReadTdfLegacyInteger(size);
            if (len == null || len.Value < 0)
                return null;

            byte[] blob = new byte[(int)len.Value];

            if (!stream.ReadAll(blob, 0, blob.Length))
                return null;

            return blob;
        }

        internal static BlazeObjectType? ReadTdfBlazeObjectType(this Stream stream)
        {
            ushort? component = (ushort?)stream.ReadTdfInteger();
            if (component == null)
                return null;

            ushort? type = (ushort?)stream.ReadTdfInteger();
            if (type == null)
                return null;

            return new BlazeObjectType(component.Value, type.Value);
        }

        internal static BlazeObjectId? ReadTdfBlazeObjectId(this Stream stream)
        {
            BlazeObjectType? type = stream.ReadTdfBlazeObjectType();
            if (type == null)
                return null;

            long? id = (long?)stream.ReadTdfInteger();
            if (id == null)
                return null;

            return new BlazeObjectId(id.Value, type.Value);
        }

        internal static float? ReadTdfFloat(this Stream stream)
        {
            byte[] temp = new byte[4];
            if (!stream.ReadAll(temp, 0, 4))
                return null;
            if (BitConverter.IsLittleEndian)
                Array.Reverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }
        internal static TdfMember? ReadTdfTag(this Stream stream) => TdfMember.FromStream(stream);
        internal static Task<TdfMember?> ReadTdfTagAsync(this Stream stream) => TdfMember.FromStreamAsync(stream);

        internal static TdfBaseType ReadTdfBaseType(this Stream stream)
        {
            int b = stream.ReadByte();
            if (b == -1)
                return TdfBaseType.TDF_TYPE_MAX;
            return (TdfBaseType)b;
        }

        internal static bool ReadTdfLegacyBaseTypeAndSize(this Stream stream, out TdfLegacyBaseType baseType, out byte size)
        {
            int typeAndSize = stream.ReadByte();
            if (typeAndSize == -1)
            {
                baseType = (TdfLegacyBaseType)255;
                size = 255;
                return false;
            }

            baseType = (TdfLegacyBaseType)(typeAndSize >> 4);
            size = (byte)(typeAndSize & 0xF);
            return true;
        }

        internal static void WriteTdfTag(this Stream stream, TdfMember tag) => stream.Write(tag.Bytes, 0, tag.Bytes.Length);
        internal static Task WriteTdfTagAsync(this Stream stream, TdfMember tag) => stream.WriteAsync(tag.Bytes, 0, tag.Bytes.Length);

        internal static void WriteTdfBaseType(this Stream stream, TdfBaseType type) => stream.WriteByte((byte)type);

        internal static void WriteTdfLegacyBaseTypeAndSize(this Stream stream, TdfLegacyBaseType baseType, int size)
        {
            byte sizeByte = size > 0xF ? (byte)0xF : (byte)size;
            stream.WriteByte((byte)(((byte)baseType << 4) | sizeByte));
            if (sizeByte == 0xF)
                stream.WriteTdfLegacyInteger(size);
        }

        internal static void WriteTdfLegacyBaseTypeAndSize(this Stream stream, TdfLegacyBaseType baseType, byte size)
        {
            stream.WriteByte((byte)(((byte)baseType << 4) | size));
        }


        internal static void WriteTdfBool(this Stream stream, bool value)
        {
            stream.WriteByte((byte)(value ? 1 : 0));
        }

        internal static void WriteTdfInteger(this Stream stream, BigInteger value)
        {
            if (value != 0)
            {
                byte curByte;

                //calculate the first byte
                if (value >= 0)
                    curByte = (byte)(value & 0x3F | 0x80); //set first six bits + pos sign bit (0) + and next bit (1)
                else
                {
                    value = -value;
                    curByte = (byte)(value & 0x3F | 0xC0); //set first six bits + neg sign bit (1) + and next bit (1)
                }

                for (BigInteger i = value >> 6; i > 0; i >>= 7)
                {
                    stream.WriteByte(curByte);
                    curByte = (byte)((i | 0x80) & 0xFF);
                }

                stream.WriteByte((byte)(curByte & 0x7F)); //change next bit to 0

            }
            else
                stream.WriteByte(0x00);
        }

        internal static void WriteTdfLegacyInteger(this Stream stream, BigInteger value)
        {
            if (value != 0)
            {
                long returnPosition = stream.Position;

                //calculate the first byte
                byte curByte = (byte)(value & 0x7F); //this is the last byte, next bit is 0
                int byteCount = 1;

                for (BigInteger i = value >> 7; i > 0; i >>= 7)
                {
                    stream.WriteByte(curByte); byteCount++;
                    curByte = (byte)((i | 0x80) & 0xFF);
                }

                stream.WriteByte(curByte);

                //for some stupid reason the bytes are reversed, so we need to fix it in stream
                byte[] bytes = new byte[byteCount];

                stream.Position = returnPosition;
                stream.Read(bytes, 0, byteCount);

                Array.Reverse(bytes);

                stream.Position = returnPosition;
                stream.Write(bytes, 0, byteCount);
            }
            else
                stream.WriteByte(0x00);
        }

        internal static void WriteTdfString(this Stream stream, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);

            stream.WriteTdfInteger(data.Length + 1);
            stream.Write(data, 0, data.Length);
            stream.WriteByte(0x00);
        }

        internal static void WriteTdfLegacyString(this Stream stream, string value, bool withType)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            int len = data.Length + 1;

            if (withType)
                stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_STRING, len);
            else
                stream.WriteTdfLegacyInteger(len);

            stream.Write(data, 0, data.Length);
            stream.WriteByte(0x00);
        }

        internal static void WriteTdfBlob(this Stream stream, byte[] value)
        {
            stream.WriteTdfInteger(value.Length);
            stream.Write(value, 0, value.Length);
        }

        internal static void WriteTdfLegacyBlob(this Stream stream, byte[] value, bool withType)
        {
            int len = value.Length;

            if (withType)
                stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_BLOB, len);
            else
                stream.WriteTdfLegacyInteger(len);

            stream.Write(value, 0, value.Length);
        }

        internal static void WriteTdfBlazeObjectType(this Stream stream, BlazeObjectType value)
        {
            stream.WriteTdfInteger(value.Component);
            stream.WriteTdfInteger(value.Type);
        }

        internal static void WriteTdfBlazeObjectId(this Stream stream, BlazeObjectId value)
        {
            stream.WriteTdfBlazeObjectType(value.Type);
            stream.WriteTdfInteger(value.Id);
        }

        internal static void WriteTdfFloat(this Stream stream, float value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(temp);
            stream.Write(temp, 0, 4);
        }

        internal static void WriteTdfTimeValue(this Stream stream, TimeValue value)
        {
            stream.WriteTdfInteger(value.Time);
        }


    }
}
