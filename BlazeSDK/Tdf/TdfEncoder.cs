using System.Collections;
using System.Numerics;
using System.Reflection;
using Tdf.Extensions;

namespace Tdf
{
    public class TdfEncoder : ITdfEncoder
    {
        private readonly TdfFactory _factory;
        private readonly bool _heat1Bug;

        internal delegate void TdfWriter(Stream stream, TdfMember tag, object value);

        internal TdfEncoder(TdfFactory factory, bool heat1Bug)
        {
            _factory = factory;
            _heat1Bug = heat1Bug;
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

            //need to encode it alphabetically
            foreach (var kvp in keyValuePairs.OrderBy(x => x.Key.Tag))
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
                    stream.WriteTdfBaseType(baseType);
                    writer(stream, tag, fieldValue);
                }
            }
        }

        private TdfWriter? GetTdfWriter(Type fieldType, TdfBaseType baseType, bool isListElement)
        {
            switch (baseType)
            {
                case TdfBaseType.TDF_TYPE_INTEGER:
                    if (fieldType == typeof(bool))
                        return WriteTdfBoolean;
                    if (fieldType == typeof(TimeValue))
                        return WriteTdfTimeValue;
                    return WriteTdfInteger;
                case TdfBaseType.TDF_TYPE_STRING:
                    return WriteTdfString;
                case TdfBaseType.TDF_TYPE_BINARY:
                    return WriteTdfBlob;
                case TdfBaseType.TDF_TYPE_STRUCT:
                    return WriteTdfStruct;
                case TdfBaseType.TDF_TYPE_LIST:
                    return WriteTdfList;
                case TdfBaseType.TDF_TYPE_MAP:
                    return WriteTdfMap;
                case TdfBaseType.TDF_TYPE_UNION:
                    if (isListElement)
                        return WriteTdfUnionAsListElement;
                    return WriteTdfUnion;
                case TdfBaseType.TDF_TYPE_VARIABLE:
                    return WriteTdfVariable;
                case TdfBaseType.TDF_TYPE_BLAZE_OBJECT_TYPE:
                    return WriteTdfBlazeObjectType;
                case TdfBaseType.TDF_TYPE_BLAZE_OBJECT_ID:
                    return WriteTdfBlazeObjectId;
                case TdfBaseType.TDF_TYPE_FLOAT:
                    return WriteTdfFloat;
                default:
                    return null;
            }
        }

        private static TdfBaseType GetTdfBaseType(Type fieldType)
        {
            switch (Type.GetTypeCode(fieldType))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return TdfBaseType.TDF_TYPE_INTEGER;
                case TypeCode.Single:
                    return TdfBaseType.TDF_TYPE_FLOAT;
                case TypeCode.String:
                    return TdfBaseType.TDF_TYPE_STRING;
            }

            if (fieldType.IsGenericType)
            {
                var genericType = fieldType.GetGenericTypeDefinition();

                if (genericType == typeof(List<>))
                    return TdfBaseType.TDF_TYPE_LIST;

                if (
                    genericType == typeof(Dictionary<,>)
                    || genericType == typeof(SortedDictionary<,>)
                )
                    return TdfBaseType.TDF_TYPE_MAP;
            }

            if (fieldType.GetCustomAttribute<TdfStruct>() != null)
                return TdfBaseType.TDF_TYPE_STRUCT;

            if (fieldType == typeof(byte[]))
                return TdfBaseType.TDF_TYPE_BINARY;

            if (fieldType == typeof(BlazeObjectType))
                return TdfBaseType.TDF_TYPE_BLAZE_OBJECT_TYPE;

            if (fieldType == typeof(BlazeObjectId))
                return TdfBaseType.TDF_TYPE_BLAZE_OBJECT_ID;

            if (fieldType.BaseType == typeof(TdfUnion))
                return TdfBaseType.TDF_TYPE_UNION;

            //NOTE: Time values are encoded as integers, TDF_TYPE_TIMEVALUE is not actually used
            return fieldType.BaseType == typeof(TimeValue) ? TdfBaseType.TDF_TYPE_INTEGER
                : fieldType == typeof(object)
                || Nullable.GetUnderlyingType(fieldType) == typeof(object)
                    ? TdfBaseType.TDF_TYPE_VARIABLE
                : TdfBaseType.TDF_TYPE_MAX;
        }

        private void WriteTdfBoolean(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfBool((bool)value);
        }

        private void WriteTdfInteger(Stream stream, TdfMember tag, object value)
        {
            var integer = Type.GetTypeCode(value.GetType()) switch
            {
                TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 => (BigInteger)
                    Convert.ToInt64(value),
                _ => (BigInteger)Convert.ToUInt64(value),
            };
            stream.WriteTdfInteger(integer);
        }

        private void WriteTdfString(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfString((string)value);
        }

        private void WriteTdfBlob(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfBlob((byte[])value);
        }

        private void WriteTdfStruct(Stream stream, TdfMember tag, object value)
        {
            WriteTo(stream, value);
            stream.WriteByte(0x00); //terminator
        }

        private void WriteTdfList(Stream stream, TdfMember tag, object value)
        {
            var listType = value.GetType().GetGenericArguments()[0];
            var baseType = GetTdfBaseType(listType);
            var writer =
                GetTdfWriter(listType, baseType, true)
                ?? throw new NotSupportedException(
                    $"List type '{listType.FullName}' not supported!"
                );

            #region bug implementation fix
            if (_heat1Bug)
            {
                if (listType.IsGenericType)
                    listType = listType.GetGenericTypeDefinition();

                if (
                    listType.BaseType == typeof(TdfUnion)
                    || listType == typeof(List<>)
                    || listType == typeof(Dictionary<,>)
                    || listType == typeof(SortedDictionary<,>)
                )
                    baseType = TdfBaseType.TDF_TYPE_STRUCT;
            }
            #endregion

            var list = (IList)value;
            stream.WriteTdfBaseType(baseType);
            stream.WriteTdfInteger(list.Count);

            foreach (var item in list)
                writer(stream, tag, item);
        }

        private void WriteTdfMap(Stream stream, TdfMember tag, object value)
        {
            var collection = (ICollection)value;
            var genericArguments = value.GetType().GetGenericArguments();
            var keyType = genericArguments[0];
            var valueType = genericArguments[1];

            var keyBaseType = GetTdfBaseType(keyType);
            var valueBaseType = GetTdfBaseType(valueType);

            var keyWriter = GetTdfWriter(keyType, keyBaseType, false);
            var valueWriter = GetTdfWriter(valueType, valueBaseType, false);

            if (keyWriter == null)
                throw new NotSupportedException(
                    $"Map key type '{keyType.FullName}' not supported!"
                );
            if (valueWriter == null)
                throw new NotSupportedException(
                    $"Map value type '{valueType.FullName}' not supported!"
                );

            stream.WriteTdfBaseType(keyBaseType);
            stream.WriteTdfBaseType(valueBaseType);
            stream.WriteTdfInteger(collection.Count);

            //item type KeyValuePair<KeyType, ValueType>
            var itemType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            var keyProperty = itemType.GetProperty("Key")!;
            var valueProperty = itemType.GetProperty("Value")!;

            foreach (var item in collection)
            {
                var kvpKey = keyProperty.GetValue(item, null)!;
                var kvpValue = valueProperty.GetValue(item, null)!;

                keyWriter(stream, tag, kvpKey);
                valueWriter(stream, tag, kvpValue);
            }
        }

        private void WriteTdfUnion(Stream stream, TdfMember tag, object value)
        {
            var union = (TdfUnion)value;

            var obj = union.GetValue();
            var activeMember = obj != null ? union.ActiveMember : (byte)0x7f;
            stream.WriteByte(activeMember);

            if (activeMember != 0x7F)
            {
                stream.Write(TdfUnion.TDF_VALU_TAG, 0, TdfUnion.TDF_VALU_TAG.Length);
                WriteTdfStruct(stream, tag, obj!);
            }
        }

        private void WriteTdfUnionAsListElement(Stream stream, TdfMember tag, object value)
        {
            var union = (TdfUnion)value;

            var obj = union.GetValue();
            var activeMember = obj != null ? union.ActiveMember : (byte)0x7f;
            stream.WriteByte(activeMember);

            if (activeMember != 0x7F)
                WriteTdfStruct(stream, tag, obj!);
        }

        private void WriteTdfVariable(Stream stream, TdfMember tag, object value)
        {
            var valueType = value.GetType(); //getting the runtime value, not the field value which is object
            var present = valueType != typeof(object);
            stream.WriteTdfBool(present);

            var tdfId = TdfFactory.GetTdfId(valueType); //if zero, then the receiving client will probably just skip the encoded variable
            stream.WriteTdfInteger(tdfId);

            var baseType = GetTdfBaseType(valueType);
            stream.WriteTdfTag(tag);
            stream.WriteTdfBaseType(baseType);

            var writer =
                GetTdfWriter(valueType, baseType, false)
                ?? throw new NotSupportedException($"Type '{valueType.FullName}' not supported!");
            writer(stream, tag, value);
            stream.WriteByte(0x00); //tdf variable terminator
        }

        private void WriteTdfBlazeObjectType(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfBlazeObjectType((BlazeObjectType)value);
        }

        private void WriteTdfBlazeObjectId(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfBlazeObjectId((BlazeObjectId)value);
        }

        private void WriteTdfFloat(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfFloat((float)value);
        }

        private void WriteTdfTimeValue(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfTimeValue((TimeValue)value);
        }
    }
}
