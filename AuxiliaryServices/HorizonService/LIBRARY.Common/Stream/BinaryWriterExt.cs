using CustomLogger;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Horizon.LIBRARY.Common.Stream
{
    public static class BinaryWriterExt
    {
        public static void Write(this BinaryWriter writer, IPAddress ip)
        {
            if (ip == null)
                writer.Write(new byte[16]);
            else
                writer.Write(Encoding.UTF8.GetBytes(ip.MapToIPv4().ToString().PadRight(16, '\0')));
        }

        public static void Write(this BinaryWriter writer, string str, int length)
        {
            if (str == null)
                writer.Write(new byte[length]);
            else if (str.Length >= length)
                writer.Write(Encoding.UTF8.GetBytes(str.Substring(0, length - 1) + "\0"));
            else
                writer.Write(Encoding.UTF8.GetBytes(str.PadRight(length, '\0')));
        }

        public static void WriteStr(this BinaryWriter writer, string str, int length)
        {
            if (str == null)
                writer.Write(new byte[length]);
            else if (str.Length >= length)
                writer.Write(Encoding.UTF8.GetBytes(str.Substring(0, length - 1) + "\0"));
            else
                writer.Write(Encoding.UTF8.GetBytes(str.PadRight(length, '\0')));
        }

        public static void Write(this BinaryWriter writer, byte[] value, int fixedLength)
        {
            if (value == null)
                // Empty, write full length
                writer.Write(new byte[fixedLength]);
            else if (value.Length > fixedLength)
                // Too large, truncate
                writer.Write(value, 0, fixedLength);
            else if (value.Length < fixedLength)
            {
                // Too small, concat
                writer.Write(value);
                writer.Write(new byte[fixedLength - value.Length]);
            }
            else
                // Just right
                writer.Write(value);
        }

        public static void Write<T>(this BinaryWriter writer, T value)
        {
            writer.WriteObject(value, typeof(T));
        }

        public static void WriteObject(this BinaryWriter writer, object value, Type type)
        {
            if (value == null || type == null)
                return;

            if (value is IStreamSerializer serializer)
                serializer.Serialize(writer);
            else if (type.IsEnum)
                writer.WriteObject(value, type.GetEnumUnderlyingType());
            else if (type == typeof(bool))
                writer.Write((bool)value);
            else if (type == typeof(byte))
                writer.Write((byte)value);
            else if (type == typeof(sbyte))
                writer.Write((sbyte)value);
            else if (type == typeof(char))
                writer.Write((char)value);
            else if (type == typeof(short))
                writer.Write((short)value);
            else if (type == typeof(ushort))
                writer.Write((ushort)value);
            else if (type == typeof(int))
                writer.Write((int)value);
            else if (type == typeof(uint))
                writer.Write((uint)value);
            else if (type == typeof(long))
                writer.Write((long)value);
            else if (type == typeof(ulong))
                writer.Write((ulong)value);
            else if (type == typeof(float))
                writer.Write((float)value);
            else if (type == typeof(double))
                writer.Write((double)value);
            else if (type == typeof(string))
                writer.Write((string)value);
            else
                LoggerAccessor.LogError("Unable to serialize object " + value + " of type " + type);
        }
    }
}
