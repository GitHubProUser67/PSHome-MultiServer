using System.Collections;
using System.Reflection;
using Tdf.Extensions;

namespace Tdf
{
    public class TdfLegacyEncoder : ITdfEncoder
    {
        private TdfFactory _factory;

        internal delegate void TdfWriter(Stream stream, TdfMember tag, object value);

        internal TdfLegacyEncoder(TdfFactory factory)
        {
            _factory = factory;
        }

        public byte[] Encode<T>(T obj) where T : notnull
        {
            using (MemoryStream payload = new MemoryStream())
            {
                WriteTo(payload, obj);
                return payload.ToArray();
            }
        }


        public byte[] Encode(object obj)
        {
            using (MemoryStream payload = new MemoryStream())
            {
                WriteTo(payload, obj);
                return payload.ToArray();
            }
        }

        public void WriteTo<T>(Stream stream, T obj) where T : notnull => WriteTo(stream, (object)obj);

        public void WriteTo(Stream stream, object obj)
        {
            Type objectType = obj.GetType();


            Dictionary<TdfMember, FieldInfo> keyValuePairs = new Dictionary<TdfMember, FieldInfo>();

            foreach (FieldInfo field in objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                TdfMember? tag = field.GetCustomAttribute<TdfMember>();
                if (tag == null) //no tag, skip it
                    continue;

                keyValuePairs.Add(tag, field);
            }

            foreach (KeyValuePair<TdfMember, FieldInfo> kvp in keyValuePairs/*.OrderBy(x => x.Key.Tag)*/)
            {
                TdfMember? tag = kvp.Key;
                FieldInfo field = kvp.Value;

                object? fieldValue = field.GetValue(obj);
                if (fieldValue == null) //no value, we skip encoding it
                    continue;

                TdfLegacyBaseType baseType = GetTdfBaseType(field.FieldType);
                TdfWriter? writer = GetTdfWriter(field.FieldType, baseType, false);

                if (writer != null)
                {
                    stream.WriteTdfTag(tag);
                    writer(stream, tag, fieldValue);
                }
            }
        }
        private TdfLegacyBaseType GetTdfBaseType(Type fieldType)
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
                Type genericType = fieldType.GetGenericTypeDefinition();

                if (genericType == typeof(List<>))
                    return TdfLegacyBaseType.TYPE_ARRAY;

                if (genericType == typeof(Dictionary<,>) || genericType == typeof(SortedDictionary<,>))
                    return TdfLegacyBaseType.TYPE_MAP;
            }

            if (fieldType.GetCustomAttribute<TdfStruct>() != null)
                return TdfLegacyBaseType.TYPE_STRUCT;

            if (fieldType == typeof(byte[]))
                return TdfLegacyBaseType.TYPE_BLOB;

            if (fieldType.BaseType == typeof(TdfUnion))
                return TdfLegacyBaseType.TYPE_UNION;

            throw new Exception("UNKNOWN BASE TYPE FOR TYPE: " + fieldType.FullName);
        }

        private TdfWriter? GetTdfWriter(Type fieldType, TdfLegacyBaseType baseType, bool withoutType)
        {
            if (withoutType)
            {
                switch (baseType)
                {
                    case TdfLegacyBaseType.TYPE_STRUCT:
                        return WriteTdfStruct;
                    case TdfLegacyBaseType.TYPE_STRING:
                        return WriteTdfString;
                    case TdfLegacyBaseType.TYPE_INT8:
                        return WriteTdfInt8;
                    case TdfLegacyBaseType.TYPE_UINT8:
                        return WriteTdfUInt8;
                    case TdfLegacyBaseType.TYPE_INT16:
                        return WriteTdfInt16;
                    case TdfLegacyBaseType.TYPE_UINT16:
                        return WriteTdfUInt16;
                    case TdfLegacyBaseType.TYPE_INT32:
                        return WriteTdfInt32;
                    case TdfLegacyBaseType.TYPE_UINT32:
                        return WriteTdfUInt32;
                    case TdfLegacyBaseType.TYPE_INT64:
                        return WriteTdfInt64;
                    case TdfLegacyBaseType.TYPE_UINT64:
                        return WriteTdfUInt64;
                    case TdfLegacyBaseType.TYPE_ARRAY:
                        return WriteTdfArray;
                    case TdfLegacyBaseType.TYPE_BLOB:
                        return WriteTdfBlob;
                    case TdfLegacyBaseType.TYPE_MAP:
                        return WriteTdfMap;
                    case TdfLegacyBaseType.TYPE_UNION:
                        return WriteTdfUnion;
                    default:
                        return null;
                }
            }


            switch (baseType)
            {
                case TdfLegacyBaseType.TYPE_STRUCT:
                    return WriteTdfStructWithType;
                case TdfLegacyBaseType.TYPE_STRING:
                    return WriteTdfStringWithType;
                case TdfLegacyBaseType.TYPE_INT8:
                    return WriteTdfInt8WithType;
                case TdfLegacyBaseType.TYPE_UINT8:
                    return WriteTdfUInt8WithType;
                case TdfLegacyBaseType.TYPE_INT16:
                    return WriteTdfInt16WithType;
                case TdfLegacyBaseType.TYPE_UINT16:
                    return WriteTdfUInt16WithType;
                case TdfLegacyBaseType.TYPE_INT32:
                    return WriteTdfInt32WithType;
                case TdfLegacyBaseType.TYPE_UINT32:
                    return WriteTdfUInt32WithType;
                case TdfLegacyBaseType.TYPE_INT64:
                    return WriteTdfInt64WithType;
                case TdfLegacyBaseType.TYPE_UINT64:
                    return WriteTdfUInt64WithType;
                case TdfLegacyBaseType.TYPE_ARRAY:
                    return WriteTdfArrayWithType;
                case TdfLegacyBaseType.TYPE_BLOB:
                    return WriteTdfBlobWithType;
                case TdfLegacyBaseType.TYPE_MAP:
                    return WriteTdfMap;
                case TdfLegacyBaseType.TYPE_UNION:
                    return WriteTdfUnionWithType;
                default:
                    return null;
            }
        }

        private byte GetDefaultTypeSize(TdfLegacyBaseType baseType)
        {
            switch (baseType)
            {
                case TdfLegacyBaseType.TYPE_STRUCT:
                    return 0;
                case TdfLegacyBaseType.TYPE_STRING:
                    return 15;
                case TdfLegacyBaseType.TYPE_INT8:
                    return sizeof(sbyte);
                case TdfLegacyBaseType.TYPE_UINT8:
                    return sizeof(byte);
                case TdfLegacyBaseType.TYPE_INT16:
                    return sizeof(short);
                case TdfLegacyBaseType.TYPE_UINT16:
                    return sizeof(ushort);
                case TdfLegacyBaseType.TYPE_INT32:
                    return sizeof(int);
                case TdfLegacyBaseType.TYPE_UINT32:
                    return sizeof(uint);
                case TdfLegacyBaseType.TYPE_INT64:
                    return sizeof(long);
                case TdfLegacyBaseType.TYPE_UINT64:
                    return sizeof(ulong);
                case TdfLegacyBaseType.TYPE_ARRAY:
                    return 1;
                case TdfLegacyBaseType.TYPE_BLOB:
                    return 15; //assumption
                case TdfLegacyBaseType.TYPE_MAP:
                    return 15; //assumption
                case TdfLegacyBaseType.TYPE_UNION:
                    return 0;
                default:
                    return 0;
            }
        }

        private void WriteTdfInt8WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_INT8, sizeof(sbyte));
            WriteTdfInt8(stream, tag, value);
        }

        private void WriteTdfInt8(Stream stream, TdfMember tag, object value)
        {
            sbyte actualVal = (sbyte)Convert.ChangeType(value, typeof(sbyte));
            stream.WriteByte(unchecked((byte)actualVal));
        }

        private void WriteTdfUInt8WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_UINT8, sizeof(byte));
            WriteTdfUInt8(stream, tag, value);
        }

        private void WriteTdfUInt8(Stream stream, TdfMember tag, object value)
        {
            byte actualVal = (byte)Convert.ChangeType(value, typeof(byte));
            stream.WriteByte(actualVal);
        }

        private void WriteTdfInt16WithType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfLegacyBaseTypeAndSize(TdfLegacyBaseType.TYPE_INT16, sizeof(short));
            WriteTdfInt16(stream, tag, value);
        }

        private void WriteTdfInt16(Stream stream, TdfMember tag, object value)
        {
            short actualVal = (short)Convert.ChangeType(value, typeof(short));
            byte[] buf = BitConverter.GetBytes(actualVal);
            if (BitConverter.IsLittleEndian)
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
            ushort actualVal = (ushort)Convert.ChangeType(value, typeof(ushort));
            byte[] buf = BitConverter.GetBytes(actualVal);
            if (BitConverter.IsLittleEndian)
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
            int actualVal = (int)Convert.ChangeType(value, typeof(int));
            byte[] buf = BitConverter.GetBytes(actualVal);
            if (BitConverter.IsLittleEndian)
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
            uint actualVal = (uint)Convert.ChangeType(value, typeof(uint));
            byte[] buf = BitConverter.GetBytes(actualVal);
            if (BitConverter.IsLittleEndian)
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
            long actualVal = (long)Convert.ChangeType(value, typeof(long));
            byte[] buf = BitConverter.GetBytes(actualVal);
            if (BitConverter.IsLittleEndian)
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
            ulong actualVal = (ulong)Convert.ChangeType(value, typeof(ulong));
            byte[] buf = BitConverter.GetBytes(actualVal);
            if (BitConverter.IsLittleEndian)
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
            IList list = (IList)value;
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
            Type listType = value.GetType().GetGenericArguments()[0];
            TdfLegacyBaseType baseType = GetTdfBaseType(listType);
            TdfWriter? writer = GetTdfWriter(listType, baseType, true);

            if (writer == null)
                throw new NotSupportedException($"List type '{listType.FullName}' not supported!");

            IList list = (IList)value;
            stream.WriteTdfLegacyInteger(list.Count);
            stream.WriteTdfLegacyBaseTypeAndSize(baseType, GetDefaultTypeSize(baseType));

            foreach (object item in list)
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
            ICollection collection = (ICollection)value;
            IEnumerator enumerator = collection.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                // Empty map, we skip encoding it entirely
                stream.Seek(-TdfMember.TAG_LENGTH, SeekOrigin.Current);
                return;
            }

            Type[] genericArguments = value.GetType().GetGenericArguments();
            Type keyType = genericArguments[0];
            Type valueType = genericArguments[1];

            TdfLegacyBaseType keyBaseType = GetTdfBaseType(keyType);
            TdfLegacyBaseType valueBaseType = GetTdfBaseType(valueType);

            TdfWriter? keyWriter = GetTdfWriter(keyType, keyBaseType, true);
            TdfWriter? valueWriter = GetTdfWriter(valueType, valueBaseType, true);

            if (keyWriter == null)
                throw new NotSupportedException($"Map key type '{keyType.FullName}' not supported!");
            if (valueWriter == null)
                throw new NotSupportedException($"Map value type '{valueType.FullName}' not supported!");

            //item type KeyValuePair<KeyType, ValueType>
            Type itemType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            PropertyInfo keyProperty = itemType.GetProperty("Key")!;
            PropertyInfo valueProperty = itemType.GetProperty("Value")!;


            object item = enumerator.Current;
            object kvpKey = keyProperty.GetValue(item, null)!;
            object kvpValue = valueProperty.GetValue(item, null)!;

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
            TdfUnion union = (TdfUnion)value;
            object? obj = union.GetValue();
            byte activeMember = obj != null ? union.ActiveMember : (byte)0x7f;
            stream.WriteByte(activeMember);

            if (activeMember != 0x7F)
            {
                stream.Write(TdfUnion.TDF_LEGACY_VALU_TAG, 0, TdfUnion.TDF_LEGACY_VALU_TAG.Length);
                WriteTdfStructWithType(stream, tag, obj!);
            }
        }


    }
}
