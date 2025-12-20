using BlazeCommon.PacketDisplayAttributes;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using Tdf;

namespace BlazeCommon
{
    public class BlazePacket<T> : IBlazePacket where T : notnull
    {
        public FireFrame Frame { get; set; }
        public T Data { get; set; }
        public object DataObj => Data;

        public BlazePacket(FireFrame frame, T data)
        {
            Frame = frame;
            Data = data;
        }


        public string ToString(IBlazeComponent component, bool inbound)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Frame.ToString(component, inbound));

            TdfStruct? tdfStruct = typeof(T).GetCustomAttribute<TdfStruct>();
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
            StringBuilder builder = new StringBuilder();

            Type objectType = obj.GetType();
            foreach (FieldInfo field in objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                TdfMember? tag = field.GetCustomAttribute<TdfMember>();
                if (tag == null)
                    continue;

                object? fieldValue = field.GetValue(obj);
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
                    string unionStr = "";
                    //Type? fieldBaseType = fieldValue.GetType().BaseType;
                    //if (fieldBaseType == typeof(TdfUnion))
                    //{
                    //    TdfUnion union = (TdfUnion)fieldValue;
                    //    unionStr = $"(union : {union.ActiveMember}) ";
                    //}
                    builder.AppendLine($"{new string(' ', spaces)}{field.Name}({tag}) {unionStr}= {_obj2str(fieldValue, field, spaces + deltaSpaces, deltaSpaces)}");
                }

            }

            return builder.ToString();
        }

        private DateTime ToDateTime(long time, TimeFormat format)
        {
            switch (format)
            {
                case TimeFormat.UnixSeconds:
                    return BlazeUtils.DateTimeFromUnixSeconds(time);
                case TimeFormat.UnixMilliseconds:
                    return BlazeUtils.DateTimeFromUnixMilliseconds(time);
                case TimeFormat.UnixMicroseconds:
                    return BlazeUtils.DateTimeFromUnixMicroseconds(time);
                default:
                    throw new InvalidOperationException($"Unknown time format {format}");
            }
        }

        private string Uint32ToString(object value, FieldInfo? fieldInfo)
        {
            uint val = (uint)value;
            if (fieldInfo != null)
            {
                DisplayAsIpAddress? displayAsIpAttribute = fieldInfo.GetCustomAttribute<DisplayAsIpAddress>();
                if (displayAsIpAttribute != null)
                    return $"\"{BlazeUtils.ToIpAddress(val)}\" ({value}) (0x{val:X8})";

                DisplayAsLocale? displayAsLocaleAttribute = fieldInfo.GetCustomAttribute<DisplayAsLocale>();
                if (displayAsLocaleAttribute != null)
                    return $"\"{BlazeUtils.ToLocaleString(val)}\" ({value}) (0x{val:X8})";

                DisplayAsDateTime? displayAsDateTimeAttribute = fieldInfo.GetCustomAttribute<DisplayAsDateTime>();
                if (displayAsDateTimeAttribute != null)
                    return $"\"{ToDateTime(val, displayAsDateTimeAttribute.Format)}\" ({value}) (0x{val:X8})";
            }

            return $"{value} (0x{val:X8})";
        }

        private string _obj2str(object obj, FieldInfo? fieldInfo, int spaces, int deltaSpaces)
        {
            Type type = obj.GetType();
            TypeCode objTypeCode = Type.GetTypeCode(type);

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
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("{");
                builder.Append(Object2String(obj, spaces + deltaSpaces, deltaSpaces));
                builder.Append($"{new string(' ', spaces - deltaSpaces)}}}");
                return builder.ToString();
            }

            if (type == typeof(byte[]))
            {
                StringBuilder builder = new StringBuilder();
                byte[] arr = (byte[])obj;
                if (arr.Length > 1024)
                    Array.Resize(ref arr, 1024);

                MemoryStream stream = new MemoryStream(arr, false);


                builder.AppendLine("{");

                string spacesStr1 = new string(' ', spaces);
                string spacesStr2 = new string(' ', spaces - deltaSpaces);

                while (stream.Position < stream.Length)
                {
                    byte[] buf = new byte[16];
                    int count = stream.Read(buf, 0, 16);
                    builder.Append(spacesStr1);

                    for (int k = 0; k < count; k++)
                        builder.Append($"{buf[k]:x2} ");

                    int missingCount = 16 - count;
                    for (int k = 0; k < missingCount; k++)
                        builder.Append("   ");


                    for (int k = 0; k < count; k++)
                        builder.Append($"{((buf[k] < 0x20 || buf[k] > 0x7e) ? '.' : (char)buf[k])}");
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
                StringBuilder builder = new StringBuilder();

                builder.AppendLine("[");

                string spacesStr1 = new string(' ', spaces);
                string spacesStr2 = new string(' ', spaces - deltaSpaces);

                ICollection? collection = obj as ICollection;
                if (collection == null)
                    throw new InvalidOperationException("List must have ICollection interface");

                int i = 0;
                foreach (object item in collection)
                {
                    builder.Append(spacesStr1);
                    builder.AppendLine($"[{i++}] = {_obj2str(item, null, spaces + deltaSpaces, deltaSpaces)}");
                }

                builder.Append($"{spacesStr2}]");
                return builder.ToString();
            }

            if (type == typeof(Dictionary<,>) || type == typeof(SortedDictionary<,>))
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendLine("[");

                string spacesStr1 = new string(' ', spaces);
                string spacesStr2 = new string(' ', spaces - deltaSpaces);

                Type mapType = obj.GetType();
                Type[] genericArguments = mapType.GetGenericArguments();
                Type mapKeyType = genericArguments[0];
                Type mapValueType = genericArguments[1];

                ICollection? collection = obj as ICollection;
                if (collection == null)
                    throw new InvalidOperationException("Map must have ICollection interface");

                //item type KeyValuePair<KeyType, ValueType>
                Type itemType = typeof(KeyValuePair<,>).MakeGenericType(mapKeyType, mapValueType);
                PropertyInfo keyProperty = itemType.GetProperty("Key")!;
                PropertyInfo valueProperty = itemType.GetProperty("Value")!;

                foreach (object item in collection)
                {
                    object kvpKey = keyProperty.GetValue(item, null)!;
                    object kvpValue = valueProperty.GetValue(item, null)!;

                    builder.Append(spacesStr1);
                    builder.AppendLine($"({_obj2str(kvpKey, null, spaces + deltaSpaces, deltaSpaces)}, {_obj2str(kvpValue, null, spaces + deltaSpaces, deltaSpaces)})");
                }

                builder.Append($"{spacesStr2}]");
                return builder.ToString();
            }

            if (type.BaseType == typeof(TdfUnion))
            {
                TdfUnion union = (TdfUnion)obj;


                StringBuilder builder = new StringBuilder();
                builder.AppendLine("{");

                object? value = union.GetValue();
                if (value != null)
                {

                    string? fieldName = union.GetValueName();

                    builder.AppendLine($"{new string(' ', spaces + deltaSpaces)}{(fieldName != null ? $"{fieldName}(VALU)" : "VALU")} (union : {union.ActiveMember}) = {{");
                    builder.Append(Object2String(value, spaces + deltaSpaces + deltaSpaces, deltaSpaces));
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
            byte[] data = encoder.Encode(Data);
            Frame.Size = (uint)data.Length;
            Frame.WriteTo(stream);
            stream.Write(data, 0, data.Length);
        }

        public async Task WriteToAsync(Stream stream, ITdfEncoder encoder)
        {
            byte[] data = encoder.Encode(Data);
            Frame.Size = (uint)data.Length;
            await Frame.WriteToAsync(stream);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public byte[] Encode(ITdfEncoder encoder)
        {
            byte[] data = encoder.Encode(Data);
            Frame.Size = (uint)data.Length;
            byte[] frame = Frame.ToHeader();
            byte[] result = new byte[frame.Length + data.Length];
            Buffer.BlockCopy(frame, 0, result, 0, frame.Length);
            Buffer.BlockCopy(data, 0, result, frame.Length, data.Length);
            return result;
        }

        public ProtoFirePacket ToProtoFirePacket(ITdfEncoder encoder)
        {
            return new ProtoFirePacket(Frame, encoder.Encode(Data));
        }

        public BlazePacket<Resp> CreateResponsePacket<Resp>(Resp data) where Resp : notnull
        {
            return new BlazePacket<Resp>(Frame.CreateResponseFrame(), data);
        }

        public BlazePacket<Resp> CreateResponsePacket<Resp>(int errorCode) where Resp : notnull
        {
            return new BlazePacket<Resp>(Frame.CreateResponseFrame(errorCode), default(Resp)!);
        }

        public BlazePacket<Resp> CreateResponsePacket<Resp>(Resp data, int errorCode) where Resp : notnull
        {
            return new BlazePacket<Resp>(Frame.CreateResponseFrame(errorCode), data);
        }

        public IBlazePacket CreateResponsePacket(object data, int errorCode)
        {
            Type fullType = typeof(BlazePacket<>).MakeGenericType(data.GetType());
            return (IBlazePacket)Activator.CreateInstance(fullType, Frame.CreateResponseFrame(errorCode), data)!;
        }

        public IBlazePacket CreateResponsePacket(int errorCode)
        {
            Type fullType = typeof(BlazePacket<>).MakeGenericType(typeof(NullStruct));
            return (IBlazePacket)Activator.CreateInstance(fullType, Frame.CreateResponseFrame(errorCode), new NullStruct())!;
        }

        public IBlazePacket CreateResponsePacket(object data)
        {
            Type fullType = typeof(BlazePacket<>).MakeGenericType(data.GetType());
            return (IBlazePacket)Activator.CreateInstance(fullType, Frame.CreateResponseFrame(), data)!;
        }

        public static implicit operator T(BlazePacket<T> packet)
        {
            return packet.Data;
        }
    }
}
