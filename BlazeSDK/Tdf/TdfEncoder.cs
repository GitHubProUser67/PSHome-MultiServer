using System.Collections;
using System.Numerics;
using System.Reflection;
using Tdf.Extensions;

namespace Tdf
{
    public class TdfEncoder : ITdfEncoder
    {
        private TdfFactory _factory;
        private bool _heat1Bug;

        internal delegate void TdfWriter(Stream stream, TdfMember tag, object value);

        internal TdfEncoder(TdfFactory factory, bool heat1Bug)
        {
            _factory = factory;
            _heat1Bug = heat1Bug;
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

            //need to encode it alphabetically
            foreach (KeyValuePair<TdfMember, FieldInfo> kvp in keyValuePairs.OrderBy(x => x.Key.Tag))
            {
                TdfMember? tag = kvp.Key;
                FieldInfo field = kvp.Value;

                object? fieldValue = field.GetValue(obj);
                if (fieldValue == null) //no value, we skip encoding it
                    continue;

                TdfBaseType baseType = GetTdfBaseType(field.FieldType);
                TdfWriter? writer = GetTdfWriter(field.FieldType, baseType, false);

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


        private TdfBaseType GetTdfBaseType(Type fieldType)
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
                Type genericType = fieldType.GetGenericTypeDefinition();

                if (genericType == typeof(List<>))
                    return TdfBaseType.TDF_TYPE_LIST;

                if (genericType == typeof(Dictionary<,>) || genericType == typeof(SortedDictionary<,>))
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
            if (fieldType.BaseType == typeof(TimeValue))
                return TdfBaseType.TDF_TYPE_INTEGER;

            if (fieldType == typeof(object) || Nullable.GetUnderlyingType(fieldType) == typeof(object))
                return TdfBaseType.TDF_TYPE_VARIABLE;

            return TdfBaseType.TDF_TYPE_MAX;
        }


        private void WriteTdfBoolean(Stream stream, TdfMember tag, object value)
        {
            stream.WriteTdfBool((bool)value);
        }

        private void WriteTdfInteger(Stream stream, TdfMember tag, object value)
        {
            BigInteger integer;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    integer = Convert.ToInt64(value);
                    break;
                default:
                    integer = Convert.ToUInt64(value);
                    break;
            }


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
            Type listType = value.GetType().GetGenericArguments()[0];
            TdfBaseType baseType = GetTdfBaseType(listType);
            TdfWriter? writer = GetTdfWriter(listType, baseType, true);

            if (writer == null)
                throw new NotSupportedException($"List type '{listType.FullName}' not supported!");

            #region bug implementation fix
            if (_heat1Bug)
            {
                if (listType.IsGenericType)
                    listType = listType.GetGenericTypeDefinition();

                if (listType.BaseType == typeof(TdfUnion) || listType == typeof(List<>) || listType == typeof(Dictionary<,>) || listType == typeof(SortedDictionary<,>))
                    baseType = TdfBaseType.TDF_TYPE_STRUCT;
            }
            #endregion

            IList list = (IList)value;
            stream.WriteTdfBaseType(baseType);
            stream.WriteTdfInteger(list.Count);

            foreach (object item in list)
                writer(stream, tag, item);
        }

        private void WriteTdfMap(Stream stream, TdfMember tag, object value)
        {
            ICollection collection = (ICollection)value;
            Type[] genericArguments = value.GetType().GetGenericArguments();
            Type keyType = genericArguments[0];
            Type valueType = genericArguments[1];

            TdfBaseType keyBaseType = GetTdfBaseType(keyType);
            TdfBaseType valueBaseType = GetTdfBaseType(valueType);

            TdfWriter? keyWriter = GetTdfWriter(keyType, keyBaseType, false);
            TdfWriter? valueWriter = GetTdfWriter(valueType, valueBaseType, false);

            if (keyWriter == null)
                throw new NotSupportedException($"Map key type '{keyType.FullName}' not supported!");
            if (valueWriter == null)
                throw new NotSupportedException($"Map value type '{valueType.FullName}' not supported!");

            stream.WriteTdfBaseType(keyBaseType);
            stream.WriteTdfBaseType(valueBaseType);
            stream.WriteTdfInteger(collection.Count);

            //item type KeyValuePair<KeyType, ValueType>
            Type itemType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            PropertyInfo keyProperty = itemType.GetProperty("Key")!;
            PropertyInfo valueProperty = itemType.GetProperty("Value")!;

            foreach (object item in collection)
            {
                object kvpKey = keyProperty.GetValue(item, null)!;
                object kvpValue = valueProperty.GetValue(item, null)!;

                keyWriter(stream, tag, kvpKey);
                valueWriter(stream, tag, kvpValue);
            }
        }

        private void WriteTdfUnion(Stream stream, TdfMember tag, object value)
        {
            TdfUnion union = (TdfUnion)value;

            object? obj = union.GetValue();
            byte activeMember = obj != null ? union.ActiveMember : (byte)0x7f;
            stream.WriteByte(activeMember);

            if (activeMember != 0x7F)
            {
                stream.Write(TdfUnion.TDF_VALU_TAG, 0, TdfUnion.TDF_VALU_TAG.Length);
                WriteTdfStruct(stream, tag, obj!);
            }
        }

        private void WriteTdfUnionAsListElement(Stream stream, TdfMember tag, object value)
        {
            TdfUnion union = (TdfUnion)value;

            object? obj = union.GetValue();
            byte activeMember = obj != null ? union.ActiveMember : (byte)0x7f;
            stream.WriteByte(activeMember);

            if (activeMember != 0x7F)
                WriteTdfStruct(stream, tag, obj!);
        }

        private void WriteTdfVariable(Stream stream, TdfMember tag, object value)
        {
            Type valueType = value.GetType(); //getting the runtime value, not the field value which is object
            bool present = valueType != typeof(object);
            stream.WriteTdfBool(present);

            uint tdfId = _factory.GetTdfId(valueType); //if zero, then the receiving client will probably just skip the encoded variable
            stream.WriteTdfInteger(tdfId);

            TdfBaseType baseType = GetTdfBaseType(valueType);
            stream.WriteTdfTag(tag);
            stream.WriteTdfBaseType(baseType);

            TdfWriter? writer = GetTdfWriter(valueType, baseType, false);
            if (writer == null)
                throw new NotSupportedException($"Type '{valueType.FullName}' not supported!");
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
