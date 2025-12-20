using System.Reflection;
using Tdf.Extensions;

namespace Tdf
{
    public class TdfLegacyDecoder : ITdfDecoder
    {
        private TdfFactory _factory;

        //return value: success reading the data (if not stop continue reading the packet)
        internal delegate bool TdfReader(Stream stream, byte size, ref object? instance, FieldInfo? field);

        internal TdfLegacyDecoder(TdfFactory factory)
        {
            _factory = factory;
        }

        public T Decode<T>(byte[] data) where T : notnull => Decode<T>(new MemoryStream(data));

        public T Decode<T>(Stream stream) where T : notnull
        {
            object? ret = Activator.CreateInstance<T>();
            if (ret == null)
                throw new NotSupportedException($"'{typeof(T).FullName}' must have a parameterless constructor!");

            Type type = typeof(T);
            Dictionary<string, FieldInfo> mainContext = _factory.GetContext(type);

            while (stream.Position < stream.Length && ReadTdf(stream, ref ret, mainContext)) ;
            return (T)ret!;
        }

        public object Decode(Type type, byte[] data) => Decode(type, new MemoryStream(data));

        public object Decode(Type type, Stream stream)
        {
            object? ret = Activator.CreateInstance(type);
            if (ret == null)
                throw new NotSupportedException($"'{type.FullName}' must have a parameterless constructor!");

            Dictionary<string, FieldInfo> mainContext = _factory.GetContext(type);

            while (stream.Position < stream.Length && ReadTdf(stream, ref ret, mainContext)) ;
            return ret!;
        }

        private bool ReadTdf(Stream stream, ref object? instance, Dictionary<string, FieldInfo> context)
        {
            TdfMember? tdfMember = stream.ReadTdfTag();
            if (tdfMember == null)
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out TdfLegacyBaseType baseType, out byte size))
                return false;

            //Console.WriteLine($"ReadTdf: {tdfMember} {baseType} {size}");

            context.TryGetValue(tdfMember, out FieldInfo? field);
            TdfReader? reader = GetTdfReader(baseType);
            if (reader == null)
                return false;
            bool res = reader(stream, size, ref instance, field);
            //Console.WriteLine($"ReadTdf: {tdfMember} {baseType} {field?.Name} {res}");
            return res;
        }

        private TdfReader? GetTdfReader(TdfLegacyBaseType baseType)
        {
            switch (baseType)
            {
                case TdfLegacyBaseType.TYPE_STRUCT:
                    return ReadTdfStruct;
                case TdfLegacyBaseType.TYPE_STRING:
                    return ReadTdfString;
                case TdfLegacyBaseType.TYPE_INT8:
                    return ReadTdfInt8;
                case TdfLegacyBaseType.TYPE_UINT8:
                    return ReadTdfUInt8;
                case TdfLegacyBaseType.TYPE_INT16:
                    return ReadTdfInt16;
                case TdfLegacyBaseType.TYPE_UINT16:
                    return ReadTdfUInt16;
                case TdfLegacyBaseType.TYPE_INT32:
                    return ReadTdfInt32;
                case TdfLegacyBaseType.TYPE_UINT32:
                    return ReadTdfUInt32;
                case TdfLegacyBaseType.TYPE_INT64:
                    return ReadTdfInt64;
                case TdfLegacyBaseType.TYPE_UINT64:
                    return ReadTdfUInt64;
                case TdfLegacyBaseType.TYPE_ARRAY:
                    return ReadTdfArray;
                case TdfLegacyBaseType.TYPE_BLOB:
                    return ReadTdfBlob;
                case TdfLegacyBaseType.TYPE_MAP:
                    return ReadTdfMap;
                case TdfLegacyBaseType.TYPE_UNION:
                    return ReadTdfUnion;
                default:
                    throw new Exception($"{baseType} is not supported!");
            }
        }

        private bool ReadTdfStruct(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            Type? type = field?.FieldType;
            if (type == null)
            {
                //object passed as a dummy type
                if (ReadTdfStruct(stream, typeof(object)) == null)
                    return false;
                return true;
            }

            object? tdfStruct = ReadTdfStruct(stream, type);
            if (tdfStruct == null)
                return false;

            field?.SetValue(instance, tdfStruct);
            return true;
        }

        private bool ReadTdfString(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            string? str = stream.ReadTdfLegacyString(size);
            if (str == null)
                return false;

            field?.SetValue(instance, str);
            return true;
        }

        private bool ReadTdfInt8(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(sbyte))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            sbyte value = unchecked((sbyte)buf[0]);
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfUInt8(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(byte))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            byte value = buf[0];
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfInt16(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(short))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf);

            short value = BitConverter.ToInt16(buf, 0);
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfUInt16(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(ushort))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf);

            ushort value = BitConverter.ToUInt16(buf, 0);
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfInt32(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(int))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf);

            int value = BitConverter.ToInt32(buf, 0);
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfUInt32(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(uint))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf);

            uint value = BitConverter.ToUInt32(buf, 0);
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfInt64(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(long))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf);

            long value = BitConverter.ToInt64(buf, 0);
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfUInt64(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(ulong))
                return false;

            byte[] buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buf);

            ulong value = BitConverter.ToUInt64(buf, 0);
            if (field != null)
            {
                object actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfArray(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            Type? listFullType = field?.FieldType;
            Type? listMemberType = listFullType?.GetGenericArguments()[0];

            ulong? countNullable = (ulong?)stream.ReadTdfLegacyInteger();
            if (countNullable == null)
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out TdfLegacyBaseType baseType, out size))
                return false;

            ulong count = countNullable.Value;
            TdfReader? reader = GetTdfReader(baseType);
            if (reader == null)
                return false;

            //unknown type, skip it
            if (listFullType == null || listMemberType == null)
            {
                object? obj = new object();
                for (ulong i = 0; i < count; i++)
                {
                    if (!reader(stream, size, ref obj, null))
                        return false;
                }
                return true;
            }

            var list = Activator.CreateInstance(listFullType);
            MethodInfo addMethod = listFullType.GetMethod("Add")!;

            object? typeContainerObj = Activator.CreateInstance(typeof(TdfValueContainer<>).MakeGenericType(listMemberType))!;
            ITdfValueContainer typeContainer = (ITdfValueContainer?)typeContainerObj!;

            for (ulong i = 0; i < count; i++)
            {
                if (!reader(stream, size, ref typeContainerObj, typeContainer.ValueFieldInfo))
                    return false;

                addMethod.Invoke(list, new[] { typeContainer.Value });
            }

            field?.SetValue(instance, list);
            return true;
        }

        private bool ReadTdfBlob(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            byte[]? blob = stream.ReadTdfLegacyBlob(size);
            if (blob == null)
                return false;

            field?.SetValue(instance, blob);
            return true;
        }

        private bool ReadTdfMap(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            ulong? countNullable = (ulong?)stream.ReadTdfLegacyInteger(size);
            if (countNullable == null)
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out TdfLegacyBaseType keyType, out byte keySize))
                return false;

            TdfLegacyBaseType valueType;
            byte valueSize;

            TdfReader? keyReader = GetTdfReader(keyType);
            if (keyReader == null)
                return false;

            ulong count = countNullable.Value;
            TdfReader? valueReader = null;

            if (field == null)
            {
                object? obj = new object();
                if (!keyReader(stream, keySize, ref obj, null)) //read first pair key
                    return false;

                if (!stream.ReadTdfLegacyBaseTypeAndSize(out valueType, out valueSize))
                    return false;

                valueReader = GetTdfReader(valueType);
                if (valueReader == null)
                    return false;
                if (!valueReader(stream, valueSize, ref obj, null)) //read first pair value
                    return false;

                for (ulong i = 1; i < count; i++) //1 pair already has been read
                {
                    if (!keyReader(stream, keySize, ref obj, null))
                        return false;
                    if (!valueReader(stream, valueSize, ref obj, null))
                        return false;
                }
                return true;
            }


            Type mapFullType = field.FieldType;
            Type[] genericArgs = mapFullType.GetGenericArguments();

            Type mapKeyType = genericArgs[0];
            object? keyContainerObj = Activator.CreateInstance(typeof(TdfValueContainer<>).MakeGenericType(mapKeyType))!;
            ITdfValueContainer keyContainer = (ITdfValueContainer?)keyContainerObj!;

            if (!keyReader(stream, keySize, ref keyContainerObj, keyContainer.ValueFieldInfo)) //read first pair key
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out valueType, out valueSize))
                return false;

            valueReader = GetTdfReader(valueType);
            if (valueReader == null)
                return false;

            Type mapValueType = genericArgs[1];
            object? valueContainerObj = Activator.CreateInstance(typeof(TdfValueContainer<>).MakeGenericType(mapValueType))!;
            ITdfValueContainer valueContainer = (ITdfValueContainer?)valueContainerObj!;

            if (!valueReader(stream, valueSize, ref valueContainerObj, valueContainer.ValueFieldInfo)) //read first pair value
                return false;

            var map = Activator.CreateInstance(mapFullType);
            MethodInfo addMethod = mapFullType.GetMethod("Add")!;

            addMethod.Invoke(map, new[] { keyContainer.Value, valueContainer.Value }); //add first pair to map


            for (ulong i = 1; i < count; i++) //1 pair already has been read
            {
                if (!keyReader(stream, keySize, ref keyContainerObj, keyContainer.ValueFieldInfo))
                    return false;
                if (!valueReader(stream, valueSize, ref valueContainerObj, valueContainer.ValueFieldInfo))
                    return false;

                addMethod.Invoke(map, new[] { keyContainer.Value, valueContainer.Value });
            }

            field?.SetValue(instance, map);
            return true;
        }

        private bool ReadTdfUnion(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            int activeMember = stream.ReadByte();
            if (activeMember == -1)
                return false;

            byte[] peek = new byte[TdfUnion.TDF_LEGACY_VALU_TAG.Length];
            if (!stream.ReadAll(peek, 0, peek.Length))
                return false;

            //don't care about the valu tag with struct datatype, now we can read this as a struct, this also fixes the bug with the list of unions
            if (!peek.SequenceEqual(TdfUnion.TDF_LEGACY_VALU_TAG))
                stream.Seek(-peek.Length, SeekOrigin.Current);
            else
            {
                byte newSize;
                if (!stream.ReadTdfLegacyBaseTypeAndSize(out TdfLegacyBaseType baseType, out newSize))
                    return false;

                if (baseType == TdfLegacyBaseType.TYPE_STRUCT)
                    size = newSize;
                else
                    stream.Seek(-(peek.Length + 1), SeekOrigin.Current);
            }

            Type? type = field?.FieldType;
            if (type == null)
            {
                //object passed as a dummy type
                if (ReadTdfStruct(stream, typeof(object)) == null)
                    return false;
                return true;
            }

            TdfUnion? union = (TdfUnion?)Activator.CreateInstance(type);
            if (union == null)
                throw new NotSupportedException($"'{type.FullName}' must have a parameterless constructor!");

            if (activeMember == 0x7f)
            {
                field?.SetValue(instance, union);
                return true;
            }

            Type? memberType = union.GetActiveMemberType((byte)activeMember);
            if (memberType == null)
            {
                //object passed as a dummy type
                if (ReadTdfStruct(stream, typeof(object)) == null)
                    return false;
                return true;
            }

            object? tdfStruct = ReadTdfStruct(stream, memberType);
            if (tdfStruct == null)
                return false;

            union.SetValue(tdfStruct);
            field?.SetValue(instance, union);
            return true;
        }



        private object? ReadTdfStruct(Stream stream, Type type)
        {
            object? structValue = Activator.CreateInstance(type);
            if (structValue == null)
                throw new NotSupportedException($"'{type.FullName}' must have a parameterless constructor!");

            Dictionary<string, FieldInfo> structContext = _factory.GetContext(type);

            int b;
            while ((b = stream.ReadByte()) != 0x00)
            {
                if (b == -1)
                    return null;
                stream.Seek(-1, SeekOrigin.Current);

                if (!ReadTdf(stream, ref structValue, structContext))
                    return null;
            }

            return structValue;
        }
    }
}
