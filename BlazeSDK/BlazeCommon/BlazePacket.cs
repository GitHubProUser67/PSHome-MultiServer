using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using BlazeCommon.PacketDisplayAttributes;
using Tdf;

namespace BlazeCommon
{
    public class BlazePacket<T>(FireFrame frame, T data) : IBlazePacket
        where T : notnull
    {
        public FireFrame Frame { get; set; } = frame;
        public T Data { get; set; } = data;
        public object DataObj => Data;

        public string ToString(IBlazeComponent component, bool inbound)
        {
            var builder = new StringBuilder();
            builder.Append(Frame.ToString(component, inbound));

            var tdfStruct = typeof(T).GetCustomAttribute<TdfStruct>();
            if (tdfStruct != null && tdfStruct.HasData)
            {
                builder.AppendLine();
                builder.AppendLine($"{typeof(T).Name} = {{");
                builder.Append(Object2String(Data, 2, 2));
                builder.Append($"}}");
            }

            return builder.ToString();
        }

        private string Object2String(object obj, int spaces, int deltaSpaces)
        {
            var builder = new StringBuilder();

            var objectType = obj.GetType();
            foreach (
                var field in objectType.GetFields(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                )
            )
            {
                var tag = field.GetCustomAttribute<TdfMember>();
                if (tag == null)
                    continue;

                var fieldValue = field.GetValue(obj);
                if (fieldValue == null) //no value, we skip it
                    continue;

                if (Attribute.GetCustomAttribute(fieldValue.GetType(), typeof(TdfStruct)) != null) //this field is a blaze struct, we have to loop it too
                {
                    builder.AppendLine($"{new string(' ', spaces)}{field.Name}({tag}) = {{");
                    builder.Append(Object2String(fieldValue, spaces + deltaSpaces, deltaSpaces));
                    builder.AppendLine($"{new string(' ', spaces)}}}");
                }
                else //field(int, list, map, etc..)
                {
                    var unionStr = "";
                    //Type? fieldBaseType = fieldValue.GetType().BaseType;
                    //if (fieldBaseType == typeof(TdfUnion))
                    //{
                    //    TdfUnion union = (TdfUnion)fieldValue;
                    //    unionStr = $"(union : {union.ActiveMember}) ";
                    //}
                    builder.AppendLine(
                        $"{new string(' ', spaces)}{field.Name}({tag}) {unionStr}= {_obj2str(fieldValue, field, spaces + deltaSpaces, deltaSpaces)}"
                    );
                }
            }

            return builder.ToString();
        }

        private static DateTime ToDateTime(long time, TimeFormat format)
        {
            return format switch
            {
                TimeFormat.UnixSeconds => BlazeUtils.DateTimeFromUnixSeconds(time),
                TimeFormat.UnixMilliseconds => BlazeUtils.DateTimeFromUnixMilliseconds(time),
                TimeFormat.UnixMicroseconds => BlazeUtils.DateTimeFromUnixMicroseconds(time),
                _ => throw new InvalidOperationException($"Unknown time format {format}"),
            };
        }

        private string Uint32ToString(object value, FieldInfo? fieldInfo)
        {
            var val = (uint)value;
            if (fieldInfo != null)
            {
                var displayAsIpAttribute = fieldInfo.GetCustomAttribute<DisplayAsIpAddress>();
                if (displayAsIpAttribute != null)
                    return $"\"{BlazeUtils.ToIpAddress(val)}\" ({value}) (0x{val:X8})";

                var displayAsLocaleAttribute = fieldInfo.GetCustomAttribute<DisplayAsLocale>();
                if (displayAsLocaleAttribute != null)
                    return $"\"{BlazeUtils.ToLocaleString(val)}\" ({value}) (0x{val:X8})";

                var displayAsDateTimeAttribute = fieldInfo.GetCustomAttribute<DisplayAsDateTime>();
                if (displayAsDateTimeAttribute != null)
                    return $"\"{BlazePacket<T>.ToDateTime(val, displayAsDateTimeAttribute.Format)}\" ({value}) (0x{val:X8})";
            }

            return $"{value} (0x{val:X8})";
        }

        private string _obj2str(object obj, FieldInfo? fieldInfo, int spaces, int deltaSpaces)
        {
            var type = obj.GetType();
            var objTypeCode = Type.GetTypeCode(type);

            switch (objTypeCode)
            {
                case TypeCode.Boolean:
                    return (bool)obj ? "true" : "false";
                case TypeCode.SByte:
                    return $"{obj} (0x{(sbyte)obj:X2})";
                case TypeCode.Byte:
                    return $"{obj} (0x{(byte)obj:X2})";
                case TypeCode.Int16:
                    return $"{obj} (0x{(short)obj:X4})";
                case TypeCode.UInt16:
                    return $"{obj} (0x{(ushort)obj:X4})";
                case TypeCode.Int32:
                    return $"{obj} (0x{(int)obj:X8})";
                case TypeCode.UInt32:
                    return Uint32ToString(obj, fieldInfo);
                case TypeCode.Int64:
                    return $"{obj} (0x{(long)obj:X16})";
                case TypeCode.UInt64:
                    return $"{obj} (0x{(ulong)obj:X16})";
                case TypeCode.String:
                    return $"\"{obj}\"";
            }

            if (Attribute.GetCustomAttribute(type, typeof(TdfStruct)) != null)
            {
                var builder = new StringBuilder();
                builder.AppendLine("{");
                builder.Append(Object2String(obj, spaces + deltaSpaces, deltaSpaces));
                builder.Append($"{new string(' ', spaces - deltaSpaces)}}}");
                return builder.ToString();
            }

            if (type == typeof(byte[]))
            {
                var builder = new StringBuilder();
                var arr = (byte[])obj;
                if (arr.Length > 1024)
                    Array.Resize(ref arr, 1024);

                var stream = new MemoryStream(arr, false);

                builder.AppendLine("{");

                var spacesStr1 = new string(' ', spaces);
                var spacesStr2 = new string(' ', spaces - deltaSpaces);

                while (stream.Position < stream.Length)
                {
                    var buf = new byte[16];
                    var count = stream.Read(buf, 0, 16);
                    builder.Append(spacesStr1);

                    for (var k = 0; k < count; k++)
                        builder.Append($"{buf[k]:x2} ");

                    var missingCount = 16 - count;
                    for (var k = 0; k < missingCount; k++)
                        builder.Append("   ");

                    for (var k = 0; k < count; k++)
                        builder.Append(
                            $"{((buf[k] < 0x20 || buf[k] > 0x7e) ? '.' : (char)buf[k])}"
                        );
                    builder.AppendLine();
                }

                builder.Append($"{spacesStr2}}}");
                return builder.ToString();
            }

            //Get rid of class generic type arguments (if they exist)
            //Example: List<string> -> List<>
            if (type.IsGenericType)
                type = type.GetGenericTypeDefinition();

            if (type == typeof(List<>))
            {
                var builder = new StringBuilder();

                builder.AppendLine("[");

                var spacesStr1 = new string(' ', spaces);
                var spacesStr2 = new string(' ', spaces - deltaSpaces);

                if (obj is not ICollection collection)
                    throw new InvalidOperationException("List must have ICollection interface");

                var i = 0;
                foreach (var item in collection)
                {
                    builder.Append(spacesStr1);
                    builder.AppendLine(
                        $"[{i++}] = {_obj2str(item, null, spaces + deltaSpaces, deltaSpaces)}"
                    );
                }

                builder.Append($"{spacesStr2}]");
                return builder.ToString();
            }

            if (type == typeof(Dictionary<,>) || type == typeof(SortedDictionary<,>))
            {
                var builder = new StringBuilder();

                builder.AppendLine("[");

                var spacesStr1 = new string(' ', spaces);
                var spacesStr2 = new string(' ', spaces - deltaSpaces);

                var mapType = obj.GetType();
                var genericArguments = mapType.GetGenericArguments();
                var mapKeyType = genericArguments[0];
                var mapValueType = genericArguments[1];

                if (obj is not ICollection collection)
                    throw new InvalidOperationException("Map must have ICollection interface");

                //item type KeyValuePair<KeyType, ValueType>
                var itemType = typeof(KeyValuePair<,>).MakeGenericType(mapKeyType, mapValueType);
                var keyProperty = itemType.GetProperty("Key")!;
                var valueProperty = itemType.GetProperty("Value")!;

                foreach (var item in collection)
                {
                    var kvpKey = keyProperty.GetValue(item, null)!;
                    var kvpValue = valueProperty.GetValue(item, null)!;

                    builder.Append(spacesStr1);
                    builder.AppendLine(
                        $"({_obj2str(kvpKey, null, spaces + deltaSpaces, deltaSpaces)}, {_obj2str(kvpValue, null, spaces + deltaSpaces, deltaSpaces)})"
                    );
                }

                builder.Append($"{spacesStr2}]");
                return builder.ToString();
            }

            if (type.BaseType == typeof(TdfUnion))
            {
                var union = (TdfUnion)obj;

                var builder = new StringBuilder();
                builder.AppendLine("{");

                var value = union.GetValue();
                if (value != null)
                {
                    var fieldName = union.GetValueName();

                    builder.AppendLine(
                        $"{new string(' ', spaces + deltaSpaces)}{(fieldName != null ? $"{fieldName}(VALU)" : "VALU")} (union : {union.ActiveMember}) = {{"
                    );
                    builder.Append(
                        Object2String(value, spaces + deltaSpaces + deltaSpaces, deltaSpaces)
                    );
                    builder.AppendLine($"{new string(' ', spaces + deltaSpaces)}}}");
                }

                builder.Append($"{new string(' ', spaces - deltaSpaces)}}}");
                return builder.ToString();
            }

            if (obj is BlazeObjectType vec2)
            {
                return vec2.ToString();
            }

            if (obj is BlazeObjectId vec3)
            {
                return vec3.ToString();
            }

            if (type == typeof(float))
            {
                //10 digits after the decimal point
                return ((float)obj).ToString("0.##########", CultureInfo.InvariantCulture);
            }

            return "TODO(" + type.Name + ")";
        }

        public void WriteTo(Stream stream, ITdfEncoder encoder)
        {
            var data = encoder.Encode(Data);
            Frame.Size = (uint)data.Length;
            Frame.WriteTo(stream);
            stream.Write(data, 0, data.Length);
        }

        public async Task WriteToAsync(Stream stream, ITdfEncoder encoder)
        {
            var data = encoder.Encode(Data);
            Frame.Size = (uint)data.Length;
            await Frame.WriteToAsync(stream).ConfigureAwait(false);
            await stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        public byte[] Encode(ITdfEncoder encoder)
        {
            var data = encoder.Encode(Data);
            Frame.Size = (uint)data.Length;
            var frame = Frame.ToHeader();
            var result = new byte[frame.Length + data.Length];
            Buffer.BlockCopy(frame, 0, result, 0, frame.Length);
            Buffer.BlockCopy(data, 0, result, frame.Length, data.Length);
            return result;
        }

        public ProtoFirePacket ToProtoFirePacket(ITdfEncoder encoder)
        {
            return new ProtoFirePacket(Frame, encoder.Encode(Data));
        }

        public BlazePacket<Resp> CreateResponsePacket<Resp>(Resp data)
            where Resp : notnull
        {
            return new BlazePacket<Resp>(Frame.CreateResponseFrame(), data);
        }

        public BlazePacket<Resp> CreateResponsePacket<Resp>(int errorCode)
            where Resp : notnull
        {
            return new BlazePacket<Resp>(Frame.CreateResponseFrame(errorCode), default!);
        }

        public BlazePacket<Resp> CreateResponsePacket<Resp>(Resp data, int errorCode)
            where Resp : notnull
        {
            return new BlazePacket<Resp>(Frame.CreateResponseFrame(errorCode), data);
        }

        public IBlazePacket CreateResponsePacket(object data, int errorCode)
        {
            var fullType = typeof(BlazePacket<>).MakeGenericType(data.GetType());
            return (IBlazePacket)
                Activator.CreateInstance(fullType, Frame.CreateResponseFrame(errorCode), data)!;
        }

        public IBlazePacket CreateResponsePacket(int errorCode)
        {
            var fullType = typeof(BlazePacket<>).MakeGenericType(typeof(NullStruct));
            return (IBlazePacket)
                Activator.CreateInstance(
                    fullType,
                    Frame.CreateResponseFrame(errorCode),
                    new NullStruct()
                )!;
        }

        public IBlazePacket CreateResponsePacket(object data)
        {
            var fullType = typeof(BlazePacket<>).MakeGenericType(data.GetType());
            return (IBlazePacket)
                Activator.CreateInstance(fullType, Frame.CreateResponseFrame(), data)!;
        }

        public static implicit operator T(BlazePacket<T> packet)
        {
            return packet.Data;
        }
    }
}
