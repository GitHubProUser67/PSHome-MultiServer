using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Tdf.Extensions;

namespace Tdf
{
    public class TdfLegacyDecoder : ITdfDecoder
    {
        private readonly TdfFactory _factory;

        //return value: success reading the data (if not stop continue reading the packet)
        internal delegate bool TdfReader(
            Stream stream,
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                byte size,
            ref object? instance,
            FieldInfo? field
        );

        internal TdfLegacyDecoder(TdfFactory factory)
        {
            _factory = factory;
        }

        public T Decode<
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                T
        >(byte[] data)
            where T : notnull => Decode<T>(new MemoryStream(data));

        public T Decode<
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                T
        >(Stream stream)
            where T : notnull
        {
            object? ret =
                Activator.CreateInstance<T>()
                ?? throw new NotSupportedException(
                    $"'{typeof(T).FullName}' must have a parameterless constructor!"
                );
            var type = typeof(T);
            var mainContext = _factory.GetContext(type);

            while (stream.Position < stream.Length && ReadTdf(stream, ref ret, mainContext))
                ;
            return (T)ret!;
        }

        public object Decode(
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type type,
            byte[] data
        ) => Decode(type, new MemoryStream(data));

        public object Decode(
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type type,
            Stream stream
        )
        {
            var ret =
                Activator.CreateInstance(type)
                ?? throw new NotSupportedException(
                    $"'{type.FullName}' must have a parameterless constructor!"
                );
            var mainContext = _factory.GetContext(type);

            while (stream.Position < stream.Length && ReadTdf(stream, ref ret, mainContext))
                ;
            return ret!;
        }

        private bool ReadTdf(
            Stream stream,
            ref object? instance,
            Dictionary<string, FieldInfo> context
        )
        {
            var tdfMember = stream.ReadTdfTag();
            if (tdfMember == null)
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out var baseType, out var size))
                return false;

            //Console.WriteLine($"ReadTdf: {tdfMember} {baseType} {size}");

            context.TryGetValue(tdfMember, out var field);
            var reader = GetTdfReader(baseType);
            if (reader == null)
                return false;
            var res = reader(stream, size, ref instance, field);
            //Console.WriteLine($"ReadTdf: {tdfMember} {baseType} {field?.Name} {res}");
            return res;
        }

        private TdfReader? GetTdfReader(TdfLegacyBaseType baseType)
        {
            return baseType switch
            {
                TdfLegacyBaseType.TYPE_STRUCT => ReadTdfStruct,
                TdfLegacyBaseType.TYPE_STRING => ReadTdfString,
                TdfLegacyBaseType.TYPE_INT8 => ReadTdfInt8,
                TdfLegacyBaseType.TYPE_UINT8 => ReadTdfUInt8,
                TdfLegacyBaseType.TYPE_INT16 => ReadTdfInt16,
                TdfLegacyBaseType.TYPE_UINT16 => ReadTdfUInt16,
                TdfLegacyBaseType.TYPE_INT32 => ReadTdfInt32,
                TdfLegacyBaseType.TYPE_UINT32 => ReadTdfUInt32,
                TdfLegacyBaseType.TYPE_INT64 => ReadTdfInt64,
                TdfLegacyBaseType.TYPE_UINT64 => ReadTdfUInt64,
                TdfLegacyBaseType.TYPE_ARRAY => ReadTdfArray,
                TdfLegacyBaseType.TYPE_BLOB => ReadTdfBlob,
                TdfLegacyBaseType.TYPE_MAP => ReadTdfMap,
                TdfLegacyBaseType.TYPE_UNION => ReadTdfUnion,
                _ => throw new Exception($"{baseType} is not supported!"),
            };
        }

        private bool ReadTdfStruct(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            var type = field?.FieldType;
            if (type == null)
            {
                //object passed as a dummy type
                return ReadTdfStruct(stream, typeof(object)) != null;
            }

            var tdfStruct = ReadTdfStruct(stream, type);
            if (tdfStruct == null)
                return false;

            field?.SetValue(instance, tdfStruct);
            return true;
        }

        private bool ReadTdfString(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            var str = stream.ReadTdfLegacyString(size);
            if (str == null)
                return false;

            field?.SetValue(instance, str);
            return true;
        }

        private bool ReadTdfInt8(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            if (size != sizeof(sbyte))
                return false;

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            var value = unchecked((sbyte)buf[0]);
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
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

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            var value = buf[0];
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
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

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);

            var value = BitConverter.ToInt16(buf, 0);
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
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

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);

            var value = BitConverter.ToUInt16(buf, 0);
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
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

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);

            var value = BitConverter.ToInt32(buf, 0);
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
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

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);

            var value = BitConverter.ToUInt32(buf, 0);
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
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

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);

            var value = BitConverter.ToInt64(buf, 0);
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
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

            var buf = new byte[size];
            if (!stream.ReadAll(buf, 0, buf.Length))
                return false;

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);

            var value = BitConverter.ToUInt64(buf, 0);
            if (field != null)
            {
                var actualValue = Convert.ChangeType(value, Type.GetTypeCode(field.FieldType));
                if (field.FieldType.IsEnum)
                    actualValue = Enum.ToObject(field.FieldType, actualValue);
                field.SetValue(instance, actualValue);
            }
            return true;
        }

        private bool ReadTdfArray(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            var listFullType = field?.FieldType;
            var listMemberType = listFullType?.GetGenericArguments()[0];

            var countNullable = (ulong?)stream.ReadTdfLegacyInteger();
            if (countNullable == null)
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out var baseType, out size))
                return false;

            var count = countNullable.Value;
            var reader = GetTdfReader(baseType);
            if (reader == null)
                return false;

            //unknown type, skip it
            if (listFullType == null || listMemberType == null)
            {
                var obj = new object();
                for (ulong i = 0; i < count; i++)
                {
                    if (!reader(stream, size, ref obj, null))
                        return false;
                }
                return true;
            }

            var list = Activator.CreateInstance(listFullType);
            var addMethod = listFullType.GetMethod("Add")!;

            var typeContainerObj = Activator.CreateInstance(
                typeof(TdfValueContainer<>).MakeGenericType(listMemberType)
            )!;
            var typeContainer = (ITdfValueContainer?)typeContainerObj!;

            for (ulong i = 0; i < count; i++)
            {
                if (!reader(stream, size, ref typeContainerObj, typeContainer.ValueFieldInfo))
                    return false;

                addMethod.Invoke(list, [typeContainer.Value]);
            }

            field?.SetValue(instance, list);
            return true;
        }

        private bool ReadTdfBlob(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            var blob = stream.ReadTdfLegacyBlob(size);
            if (blob == null)
                return false;

            field?.SetValue(instance, blob);
            return true;
        }

        private bool ReadTdfMap(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            var countNullable = (ulong?)stream.ReadTdfLegacyInteger(size);
            if (countNullable == null)
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out var keyType, out var keySize))
                return false;

            TdfLegacyBaseType valueType;
            byte valueSize;

            var keyReader = GetTdfReader(keyType);
            if (keyReader == null)
                return false;

            var count = countNullable.Value;
            TdfReader? valueReader = null;

            if (field == null)
            {
                var obj = new object();
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

            var mapFullType = field.FieldType;
            var genericArgs = mapFullType.GetGenericArguments();

            var mapKeyType = genericArgs[0];
            var keyContainerObj = Activator.CreateInstance(
                typeof(TdfValueContainer<>).MakeGenericType(mapKeyType)
            )!;
            var keyContainer = (ITdfValueContainer?)keyContainerObj!;

            if (!keyReader(stream, keySize, ref keyContainerObj, keyContainer.ValueFieldInfo)) //read first pair key
                return false;

            if (!stream.ReadTdfLegacyBaseTypeAndSize(out valueType, out valueSize))
                return false;

            valueReader = GetTdfReader(valueType);
            if (valueReader == null)
                return false;

            var mapValueType = genericArgs[1];
            var valueContainerObj = Activator.CreateInstance(
                typeof(TdfValueContainer<>).MakeGenericType(mapValueType)
            )!;
            var valueContainer = (ITdfValueContainer?)valueContainerObj!;

            if (
                !valueReader(
                    stream,
                    valueSize,
                    ref valueContainerObj,
                    valueContainer.ValueFieldInfo
                )
            ) //read first pair value
                return false;

            var map = Activator.CreateInstance(mapFullType);
            var addMethod = mapFullType.GetMethod("Add")!;

            addMethod.Invoke(map, [keyContainer.Value, valueContainer.Value]); //add first pair to map

            for (ulong i = 1; i < count; i++) //1 pair already has been read
            {
                if (!keyReader(stream, keySize, ref keyContainerObj, keyContainer.ValueFieldInfo))
                    return false;
                if (
                    !valueReader(
                        stream,
                        valueSize,
                        ref valueContainerObj,
                        valueContainer.ValueFieldInfo
                    )
                )
                    return false;

                addMethod.Invoke(map, [keyContainer.Value, valueContainer.Value]);
            }

            field?.SetValue(instance, map);
            return true;
        }

        private bool ReadTdfUnion(Stream stream, byte size, ref object? instance, FieldInfo? field)
        {
            var activeMember = stream.ReadByte();
            if (activeMember == -1)
                return false;

            var peek = new byte[TdfUnion.TDF_LEGACY_VALU_TAG.Length];
            if (!stream.ReadAll(peek, 0, peek.Length))
                return false;

            //don't care about the valu tag with struct datatype, now we can read this as a struct, this also fixes the bug with the list of unions
            if (!peek.SequenceEqual(TdfUnion.TDF_LEGACY_VALU_TAG))
                stream.Seek(-peek.Length, SeekOrigin.Current);
            else
            {
                if (!stream.ReadTdfLegacyBaseTypeAndSize(out var baseType, out var newSize))
                    return false;

                if (baseType == TdfLegacyBaseType.TYPE_STRUCT)
                    _ = newSize;
                else
                    stream.Seek(-(peek.Length + 1), SeekOrigin.Current);
            }

            var type = field?.FieldType;
            if (type == null)
            {
                //object passed as a dummy type
                return ReadTdfStruct(stream, typeof(object)) != null;
            }

            var union =
                (TdfUnion?)Activator.CreateInstance(type)
                ?? throw new NotSupportedException(
                    $"'{type.FullName}' must have a parameterless constructor!"
                );
            if (activeMember == 0x7f)
            {
                field?.SetValue(instance, union);
                return true;
            }

            var memberType = union.GetActiveMemberType((byte)activeMember);
            if (memberType == null)
            {
                //object passed as a dummy type
                return ReadTdfStruct(stream, typeof(object)) != null;
            }

            var tdfStruct = ReadTdfStruct(stream, memberType);
            if (tdfStruct == null)
                return false;

            union.SetValue(tdfStruct);
            field?.SetValue(instance, union);
            return true;
        }

        private object? ReadTdfStruct(
            Stream stream,
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type type
        )
        {
            var structValue =
                Activator.CreateInstance(type)
                ?? throw new NotSupportedException(
                    $"'{type.FullName}' must have a parameterless constructor!"
                );
            var structContext = _factory.GetContext(type);

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
