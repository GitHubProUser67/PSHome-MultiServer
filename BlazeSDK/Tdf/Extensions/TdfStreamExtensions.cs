using System.Numerics;
using System.Text;

namespace Tdf.Extensions
{
    internal static class TdfStreamExtensions
    {
        internal static BigInteger? ReadTdfInteger(this Stream stream)
        {
            BigInteger b,
                value = stream.ReadByte();

            if (value == -1)
                return null;

            byte i = 6;

            var negative = (value & 0x40) != 0;
            var readNext = (value & 0x80) != 0;
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

            return negative
                ? value != 0
                    ? -value
                    : long.MinValue
                : value;
        }

        internal static BigInteger? ReadTdfLegacyInteger(this Stream stream, byte size)
        {
            return size < 15 ? (BigInteger?)size : stream.ReadTdfLegacyInteger();
        }

        internal static BigInteger? ReadTdfLegacyInteger(this Stream stream)
        {
            BigInteger b,
                value = stream.ReadByte();
            if (value == -1)
                return null;

            var readNext = (value & 0x80) != 0;
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
            var data = stream.ReadTdfBlob();
            if (data == null)
                return null;

            //checking whether we should include last char in the string or not
            var len = data.Length;
            if (len > 0)
            {
                var lengthWithoutTrailingByte = len - 1;
                if (data[lengthWithoutTrailingByte] == 0x00)
                    len = lengthWithoutTrailingByte;
            }

            return Encoding.UTF8.GetString(data, 0, len);
        }

        internal static string? ReadTdfLegacyString(this Stream stream, byte size)
        {
            var data = stream.ReadTdfLegacyBlob(size);
            if (data == null)
                return null;

            //checking whether we should include last char in the string or not
            var len = data.Length;
            if (len > 0)
            {
                var lengthWithoutTrailingByte = len - 1;
                if (data[lengthWithoutTrailingByte] == 0x00)
                    len = lengthWithoutTrailingByte;
            }

            return Encoding.UTF8.GetString(data, 0, len);
        }

        internal static byte[]? ReadTdfBlob(this Stream stream)
        {
            var len = stream.ReadTdfInteger();
            if (len == null || len.Value < 0)
                return null;

            var blob = new byte[(int)len.Value];

            return !stream.ReadAll(blob, 0, blob.Length) ? null : blob;
        }

        internal static byte[]? ReadTdfLegacyBlob(this Stream stream, byte size)
        {
            var len = stream.ReadTdfLegacyInteger(size);
            if (len == null || len.Value < 0)
                return null;

            var blob = new byte[(int)len.Value];

            return !stream.ReadAll(blob, 0, blob.Length) ? null : blob;
        }

        internal static BlazeObjectType? ReadTdfBlazeObjectType(this Stream stream)
        {
            var component = (ushort?)stream.ReadTdfInteger();
            if (component == null)
                return null;

            var type = (ushort?)stream.ReadTdfInteger();
            return type == null ? null : new BlazeObjectType(component.Value, type.Value);
        }

        internal static BlazeObjectId? ReadTdfBlazeObjectId(this Stream stream)
        {
            var type = stream.ReadTdfBlazeObjectType();
            if (type == null)
                return null;

            var id = (long?)stream.ReadTdfInteger();
            return id == null ? null : new BlazeObjectId(id.Value, type.Value);
        }

        internal static float? ReadTdfFloat(this Stream stream)
        {
            var temp = new byte[4];
            if (!stream.ReadAll(temp, 0, 4))
                return null;
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        internal static TdfMember? ReadTdfTag(this Stream stream) => TdfMember.FromStream(stream);

        internal static Task<TdfMember?> ReadTdfTagAsync(this Stream stream) =>
            TdfMember.FromStreamAsync(stream);

        internal static TdfBaseType ReadTdfBaseType(this Stream stream)
        {
            var b = stream.ReadByte();
            return b == -1 ? TdfBaseType.TDF_TYPE_MAX : (TdfBaseType)b;
        }

        internal static bool ReadTdfLegacyBaseTypeAndSize(
            this Stream stream,
            out TdfLegacyBaseType baseType,
            out byte size
        )
        {
            var typeAndSize = stream.ReadByte();
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

        internal static void WriteTdfTag(this Stream stream, TdfMember tag) =>
            stream.Write(tag.Bytes, 0, tag.Bytes.Length);

        internal static Task WriteTdfTagAsync(this Stream stream, TdfMember tag) =>
            stream.WriteAsync(tag.Bytes, 0, tag.Bytes.Length);

        internal static void WriteTdfBaseType(this Stream stream, TdfBaseType type) =>
            stream.WriteByte((byte)type);

        internal static void WriteTdfLegacyBaseTypeAndSize(
            this Stream stream,
            TdfLegacyBaseType baseType,
            int size
        )
        {
            var sizeByte = size > 0xF ? (byte)0xF : (byte)size;
            stream.WriteByte((byte)(((byte)baseType << 4) | sizeByte));
            if (sizeByte == 0xF)
                stream.WriteTdfLegacyInteger(size);
        }

        internal static void WriteTdfLegacyBaseTypeAndSize(
            this Stream stream,
            TdfLegacyBaseType baseType,
            byte size
        )
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
                    curByte = (byte)((value & 0x3F) | 0x80); //set first six bits + pos sign bit (0) + and next bit (1)
                else
                {
                    value = -value;
                    curByte = (byte)((value & 0x3F) | 0xC0); //set first six bits + neg sign bit (1) + and next bit (1)
                }

                for (var i = value >> 6; i > 0; i >>= 7)
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
                var returnPosition = stream.Position;

                //calculate the first byte
                var curByte = (byte)(value & 0x7F); //this is the last byte, next bit is 0
                var byteCount = 1;

                for (var i = value >> 7; i > 0; i >>= 7)
                {
                    stream.WriteByte(curByte);
                    byteCount++;
                    curByte = (byte)((i | 0x80) & 0xFF);
                }

                stream.WriteByte(curByte);

                //for some stupid reason the bytes are reversed, so we need to fix it in stream
                var bytes = new byte[byteCount];

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
            var data = Encoding.UTF8.GetBytes(value);

            stream.WriteTdfInteger(data.Length + 1);
            stream.Write(data, 0, data.Length);
            stream.WriteByte(0x00);
        }

        internal static void WriteTdfLegacyString(this Stream stream, string value, bool withType)
        {
            var data = Encoding.UTF8.GetBytes(value);
            var len = data.Length + 1;

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
            var len = value.Length;

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
            var temp = BitConverter.GetBytes(value);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(temp);
            stream.Write(temp, 0, 4);
        }

        internal static void WriteTdfTimeValue(this Stream stream, TimeValue value)
        {
            stream.WriteTdfInteger(value.Time);
        }
    }
}
