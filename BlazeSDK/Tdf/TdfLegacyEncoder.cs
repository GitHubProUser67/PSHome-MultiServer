using System.Collections;
using System.Reflection;
using Tdf.Extensions;

namespace Tdf
{
    public class TdfLegacyEncoder : ITdfEncoder
    {
        private readonly TdfFactory _factory;

        internal delegate void TdfWriter(Stream stream, TdfMember tag, object value);

        internal TdfLegacyEncoder(TdfFactory factory)
        {
            _factory = factory;
        }

        public byte[] Encode<T>(T obj)
            where T : notnull
        {
            using (var payload = new MemoryStream())
            {
                WriteTo(payload, obj);
                return payload.ToArray();
            }
        }

        public byte[] Encode(object obj)
        {
            using (var payload = new MemoryStream())
            {
                WriteTo(payload, obj);
                return payload.ToArray();
            }
        }

        public void WriteTo<T>(Stream stream, T obj)
            where T : notnull => WriteTo(stream, (object)obj);

        public void WriteTo(Stream stream, object obj)
        {
            var objectType = obj.GetType();

            Dictionary<TdfMember, FieldInfo> keyValuePairs = [];

            foreach (
                var field in objectType.GetFields(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                )
            )
            {
                var tag = field.GetCustomAttribute<TdfMember>();
                if (tag == null) //no tag, skip it
                    continue;

                keyValuePairs.Add(tag, field);
            }

            foreach (
                var kvp in keyValuePairs /*.OrderBy(x => x.Key.Tag)*/
            )
            {
                var tag = kvp.Key;
                var field = kvp.Value;

                var fieldValue = field.GetValue(obj);
                if (fieldValue == null) //no value, we skip encoding it
                    continue;

                var baseType = GetTdfBaseType(field.FieldType);
                var writer = GetTdfWriter(field.FieldType, baseType, false);

                if (writer != null)
                {
                    stream.WriteTdfTag(tag);
                    writer(stream, tag, fieldValue);
                }
            }
        }

        private static TdfLegacyBaseType GetTdfBaseType(Type fieldType)
        {
            switch (Type.GetTypeCode(fieldType))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                    return TdfLegacyBaseType.TYPE_INT8;
                case TypeCode.Byte:
                    return TdfLegacyBaseType.TYPE_UINT8;
                case TypeCode.Int16:
                    return TdfLegacyBaseType.TYPE_INT16;
                case TypeCode.UInt16:
                    return TdfLegacyBaseType.TYPE_UINT16;
                case TypeCode.Int32:
                    return TdfLegacyBaseType.TYPE_INT32;
                case TypeCode.UInt32:
                    return TdfLegacyBaseType.TYPE_UINT32;
                case TypeCode.Int64:
                    return TdfLegacyBaseType.TYPE_INT64;
                case TypeCode.UInt64:
                    return TdfLegacyBaseType.TYPE_UINT64;
                case TypeCode.String:
                    return TdfLegacyBaseType.TYPE_STRING;
            }

            if (fieldType.IsGenericType)
            {
                var genericType = fieldType.GetGenericTypeDefinition();

                if (genericType == typeof(List<>))
                    return TdfLegacyBaseType.TYPE_ARRAY;

                if (
                    genericType == typeof(Dictionary<,>)
                    || genericType == typeof(SortedDictionary<,>)
                )
                    return TdfLegacyBaseType.TYPE_MAP;
            }

            return fieldType.GetCustomAttribute<TdfStruct>() != null ? TdfLegacyBaseType.TYPE_STRUCT
                : fieldType == typeof(byte[]) ? TdfLegacyBaseType.TYPE_BLOB
                : fieldType.BaseType == typeof(TdfUnion) ? TdfLegacyBaseType.TYPE_UNION
                : throw new Exception("UNKNOWN BASE TYPE FOR TYPE: " + fieldType.FullName);
        }

        private TdfWriter? GetTdfWriter(
            Type fieldType,
            TdfLegacyBaseType baseType,
            bool withoutType
        )
        {
            return withoutType
                ? baseType switch
                {
                    TdfLegacyBaseType.TYPE_STRUCT => WriteTdfStruct,
                    TdfLegacyBaseType.TYPE_STRING => WriteTdfString,
                    TdfLegacyBaseType.TYPE_INT8 => WriteTdfInt8,
                    TdfLegacyBaseType.TYPE_UINT8 => WriteTdfUInt8,
                    TdfLegacyBaseType.TYPE_INT16 => WriteTdfInt16,
                    TdfLegacyBaseType.TYPE_UINT16 => WriteTdfUInt16,
                    TdfLegacyBaseType.TYPE_INT32 => WriteTdfInt32,
                    TdfLegacyBaseType.TYPE_UINT32 => WriteTdfUInt32,
                    TdfLegacyBaseType.TYPE_INT64 => WriteTdfInt64,
                    TdfLegacyBaseType.TYPE_UINT64 => WriteTdfUInt64,
                    TdfLegacyBaseType.TYPE_ARRAY => WriteTdfArray,
                    TdfLegacyBaseType.TYPE_BLOB => WriteTdfBlob,
                    TdfLegacyBaseType.TYPE_MAP => WriteTdfMap,
                    TdfLegacyBaseType.TYPE_UNION => WriteTdfUnion,
                    _ => null,
                }
                : baseType switch
                {
                    TdfLegacyBaseType.TYPE_STRUCT => WriteTdfStructWithType,
                    TdfLegacyBaseType.TYPE_STRING => WriteTdfStringWithType,
                    TdfLegacyBaseType.TYPE_INT8 => WriteTdfInt8WithType,
                    TdfLegacyBaseType.TYPE_UINT8 => WriteTdfUInt8WithType,
                    TdfLegacyBaseType.TYPE_INT16 => WriteTdfInt16WithType,
                    TdfLegacyBaseType.TYPE_UINT16 => WriteTdfUInt16WithType,
                    TdfLegacyBaseType.TYPE_INT32 => WriteTdfInt32WithType,
                    TdfLegacyBaseType.TYPE_UINT32 => WriteTdfUInt32WithType,
                    TdfLegacyBaseType.TYPE_INT64 => WriteTdfInt64WithType,
                    TdfLegacyBaseType.TYPE_UINT64 => WriteTdfUInt64WithType,
                    TdfLegacyBaseType.TYPE_ARRAY => WriteTdfArrayWithType,
                    TdfLegacyBaseType.TYPE_BLOB => WriteTdfBlobWithType,
                    TdfLegacyBaseType.TYPE_MAP => WriteTdfMap,
                    TdfLegacyBaseType.TYPE_UNION => WriteTdfUnionWithType,
                    _ => null,
                };
        }

        private static byte GetDefaultTypeSize(TdfLegacyBaseType baseType)
        {
            return baseType switch
            {
                TdfLegacyBaseType.TYPE_STRUCT => 0,
                TdfLegacyBaseType.TYPE_STRING => 15,
                TdfLegacyBaseType.TYPE_INT8 => sizeof(sbyte),
                TdfLegacyBaseType.TYPE_UINT8 => sizeof(byte),
                TdfLegacyBaseType.TYPE_INT16 => sizeof(short),
                TdfLegacyBaseType.TYPE_UINT16 => sizeof(ushort),
                TdfLegacyBaseType.TYPE_INT32 => sizeof(int),
                TdfLegacyBaseType.TYPE_UINT32 => sizeof(uint),
                TdfLegacyBaseType.TYPE_INT64 => sizeof(long),
                TdfLegacyBaseType.TYPE_UINT64 => sizeof(ulong),
                TdfLegacyBaseType.TYPE_ARRAY => 1,
                TdfLegacyBaseType.TYPE_BLOB => 15, //assumption
                TdfLegacyBaseType.TYPE_MAP => 15, //assumption
                TdfLegacyBaseType.TYPE_UNION => 0,
                _ => 0,
            };
        }

        private void WriteTdfInt8WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_INT8, sizeof(sbyte));
            WriteTdfInt8(stream, tag, value);
        }

        private void WriteTdfInt8(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (sbyte)Convert.ChangeType(value, typeof(sbyte));
            stream.WriteByte(unchecked((byte)actualVal));
        }

        private void WriteTdfUInt8WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_UINT8, sizeof(byte));
            WriteTdfUInt8(stream, tag, value);
        }

        private void WriteTdfUInt8(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (byte)Convert.ChangeType(value, typeof(byte));
            stream.WriteByte(actualVal);
        }

        private void WriteTdfInt16WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_INT16, sizeof(short));
            WriteTdfInt16(stream, tag, value);
        }

        private void WriteTdfInt16(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (short)Convert.ChangeType(value, typeof(short));
            var buf = BitConverter.GetBytes(actualVal);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);
            stream.Write(buf, 0, sizeof(short));
        }

        private void WriteTdfUInt16WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_UINT16, sizeof(ushort));
            WriteTdfUInt16(stream, tag, value);
        }

        private void WriteTdfUInt16(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (ushort)Convert.ChangeType(value, typeof(ushort));
            var buf = BitConverter.GetBytes(actualVal);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);
            stream.Write(buf, 0, sizeof(ushort));
        }

        private void WriteTdfInt32WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_INT32, sizeof(int));
            WriteTdfInt32(stream, tag, value);
        }

        private void WriteTdfInt32(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (int)Convert.ChangeType(value, typeof(int));
            var buf = BitConverter.GetBytes(actualVal);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);
            stream.Write(buf, 0, sizeof(int));
        }

        private void WriteTdfUInt32WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_UINT32, sizeof(uint));
            WriteTdfUInt32(stream, tag, value);
        }

        private void WriteTdfUInt32(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (uint)Convert.ChangeType(value, typeof(uint));
            var buf = BitConverter.GetBytes(actualVal);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);
            stream.Write(buf, 0, sizeof(uint));
        }

        private void WriteTdfInt64WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_INT64, sizeof(long));
            WriteTdfInt64(stream, tag, value);
        }

        private void WriteTdfInt64(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (long)Convert.ChangeType(value, typeof(long));
            var buf = BitConverter.GetBytes(actualVal);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);
            stream.Write(buf, 0, sizeof(long));
        }

        private void WriteTdfUInt64WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_UINT64, sizeof(ulong));
            WriteTdfUInt64(stream, tag, value);
        }

        private void WriteTdfUInt64(Stream stream, TdfMember tag, object value)
        {
            var actualVal = (ulong)Convert.ChangeType(value, typeof(ulong));
            var buf = BitConverter.GetBytes(actualVal);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(buf);
            stream.Write(buf, 0, sizeof(ulong));
        }

        private void WriteTdfStructWithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_STRUCT, 0);
            WriteTdfStruct(stream, tag, value);
        }

        private void WriteTdfStruct(Stream stream, TdfMember tag, object value)
        {
            WriteTo(stream, value);
            stream.WriteByte(0x00); //terminator
        }

        private void WriteTdfStringWithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyString((string)value, true);
        }

        private void WriteTdfString(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyString((string)value, false);
        }

        private void WriteTdfArrayWithType(Stream stream, TdfMember tag, object value)
        {
            var list = (IList)value;
            if (list.Count == 0)
            {
                // Empty list, we skip encoding it entirely
                stream.Seek(-TdfMember.TAG_LENGTH, SeekOrigin.Current);
                return;
            }

            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_ARRAY, 1);
            WriteTdfArray(stream, tag, value);
        }

        private void WriteTdfArray(Stream stream, TdfMember tag, object value)
        {
            var listType = value.GetType().GetGenericArguments()[0];
            var baseType = GetTdfBaseType(listType);
            var writer =
                GetTdfWriter(listType, baseType, true)
                ?? throw new NotSupportedException(
                    $"List type '{listType.FullName}' not supported!"
                );
            var list = (IList)value;
            stream.WriteTdfLegacyInteger(list.Count);
            stream.WriteTdfLegacyBaseTypeAndSize(baseType, GetDefaultTypeSize(baseType));

            foreach (var item in list)
                writer(stream, tag, item);
        }

        private void WriteTdfBlobWithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBlob((byte[])value, true);
        }

        private void WriteTdfBlob(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBlob((byte[])value, false);
        }

        private void WriteTdfMap(Stream stream, TdfMember tag, object value)
        {
            var collection = (ICollection)value;
            var enumerator = collection.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                // Empty map, we skip encoding it entirely
                stream.Seek(-TdfMember.TAG_LENGTH, SeekOrigin.Current);
                return;
            }

            var genericArguments = value.GetType().GetGenericArguments();
            var keyType = genericArguments[0];
            var valueType = genericArguments[1];

            var keyBaseType = GetTdfBaseType(keyType);
            var valueBaseType = GetTdfBaseType(valueType);

            var keyWriter = GetTdfWriter(keyType, keyBaseType, true);
            var valueWriter = GetTdfWriter(valueType, valueBaseType, true);

            if (keyWriter == null)
                throw new NotSupportedException(
                    $"Map key type '{keyType.FullName}' not supported!"
                );
            if (valueWriter == null)
                throw new NotSupportedException(
                    $"Map value type '{valueType.FullName}' not supported!"
                );

            //item type KeyValuePair<KeyType, ValueType>
            var itemType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            var keyProperty = itemType.GetProperty("Key")!;
            var valueProperty = itemType.GetProperty("Value")!;

            var item = enumerator.Current;
            var kvpKey = keyProperty.GetValue(item, null)!;
            var kvpValue = valueProperty.GetValue(item, null)!;

            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_MAP, collection.Count);
            stream.WriteTdfLegacyBaseTypeAndSize(keyBaseType, GetDefaultTypeSize(keyBaseType));
            keyWriter(stream, tag, kvpKey);
            stream.WriteTdfLegacyBaseTypeAndSize(valueBaseType, GetDefaultTypeSize(valueBaseType));
            valueWriter(stream, tag, kvpValue);

            while (enumerator.MoveNext())
            {
                item = enumerator.Current;

                kvpKey = keyProperty.GetValue(item, null)!;
                kvpValue = valueProperty.GetValue(item, null)!;

                keyWriter(stream, tag, kvpKey);
                valueWriter(stream, tag, kvpValue);
            }
        }

        private void WriteTdfUnionWithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_UNION, 0);
            WriteTdfUnion(stream, tag, value);
        }

        private void WriteTdfUnion(Stream stream, TdfMember tag, object value)
        {
            var union = (TdfUnion)value;
            var obj = union.GetValue();
            var activeMember = obj != null ? union.ActiveMember : (byte)0x7f;
            stream.WriteByte(activeMember);

            if (activeMember != 0x7F)
            {
                stream.Write(TdfUnion.TDF_LEGACY_VALU_TAG, 0, TdfUnion.TDF_LEGACY_VALU_TAG.Length);
                WriteTdfStructWithType(stream, tag, obj!);
            }
        }
    }
}
