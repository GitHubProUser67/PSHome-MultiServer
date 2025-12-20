using System.Buffers.Binary;
using System.Numerics;
using System.Reflection;
using Tdf.Extensions;

namespace Tdf
{
    public class TdfDecoder : ITdfDecoder
    {
        private TdfFactory _factory;
        private bool _heat1Bug;

        //return value: success reading the data(if not stop continue reading the packet)
        internal delegate bool TdfReader(Stream stream, ref object? instance, FieldInfo? field);

        internal TdfDecoder(TdfFactory factory, bool heat1Bug)
        {
            _factory = factory;
            _heat1Bug = heat1Bug;
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

            TdfBaseType baseType = stream.ReadTdfBaseType();
            context.TryGetValue(tdfMember, out FieldInfo? field);
            TdfReader? reader = GetTdfReader(baseType);
            if (reader == null)
                return false;
            bool res = reader(stream, ref instance, field);
            //Console.WriteLine($"ReadTdf: {tdfMember} {baseType} {field?.Name} {res}");
            return res;
        }



        private TdfReader? GetTdfReader(TdfBaseType baseType)
        {
            switch (baseType)
            {
                case TdfBaseType.TDF_TYPE_INTEGER:
                    return ReadTdfInteger;
                case TdfBaseType.TDF_TYPE_STRING:
                    return ReadTdfString;
                case TdfBaseType.TDF_TYPE_BINARY:
                    return ReadTdfBlob;
                case TdfBaseType.TDF_TYPE_STRUCT:
                    return ReadTdfStruct;
                case TdfBaseType.TDF_TYPE_LIST:
                    return ReadTdfList;
                case TdfBaseType.TDF_TYPE_MAP:
                    return ReadTdfMap;
                case TdfBaseType.TDF_TYPE_UNION:
                    return ReadTdfUnion;
                case TdfBaseType.TDF_TYPE_VARIABLE:
                    return ReadTdfVariable;
                case TdfBaseType.TDF_TYPE_BLAZE_OBJECT_TYPE:
                    return ReadTdfBlazeObjectType;
                case TdfBaseType.TDF_TYPE_BLAZE_OBJECT_ID:
                    return ReadTdfBlazeObjectId;
                case TdfBaseType.TDF_TYPE_FLOAT:
                    return ReadTdfFloat;
                case TdfBaseType.TDF_TYPE_TIMEVALUE:
                    return ReadTdfTimeValue;
                default:
                    return null;
            }
        }



        private bool ReadTdfInteger(Stream stream, ref object? instance, FieldInfo? field)
        {
            BigInteger? value = stream.ReadTdfInteger();
            if (value == null)
                return false;

            if (field == null)
                return true;

            //will not be using BigInteger explicit conversions to needed data types, because it is checking whether the number fits the data type and will throw exception if it does not.
            //logic changed due to multiple encounters of exceptions when ea blaze sent a negative number for uint64 field (game protocol version hash) (it had the correct datatype).
            //it looks like they messed up with encoding uint64 where the MSB (which is the sign bit for int64) was set to 1 and therefore was interpreted as negative number (this makes sense because they internally are using int64 for encoding integers).
            //this is the reason why we are forced to do unchecked conversions, but the result still will be valid.

            byte[] binary = value.Value.ToByteArray(false, false);

            Type type = field.FieldType;
            object resObject;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    resObject = value.Value != 0;
                    break;
                case TypeCode.SByte:
                    resObject = unchecked((sbyte)binary[0]);
                    break;
                case TypeCode.Byte:
                    resObject = binary[0];
                    break;
                case TypeCode.Int16:
                    Array.Resize(ref binary, 2);
                    resObject = BinaryPrimitives.ReadInt16LittleEndian(binary);
                    break;
                case TypeCode.UInt16:
                    Array.Resize(ref binary, 2);
                    resObject = BinaryPrimitives.ReadUInt16LittleEndian(binary);
                    break;
                case TypeCode.Int32:
                    Array.Resize(ref binary, 4);
                    resObject = BinaryPrimitives.ReadInt32LittleEndian(binary);
                    break;
                case TypeCode.UInt32:
                    Array.Resize(ref binary, 4);
                    resObject = BinaryPrimitives.ReadUInt32LittleEndian(binary);
                    break;
                case TypeCode.Int64:
                    Array.Resize(ref binary, 8);
                    resObject = BinaryPrimitives.ReadInt64LittleEndian(binary);
                    break;
                case TypeCode.UInt64:
                    Array.Resize(ref binary, 8);
                    resObject = BinaryPrimitives.ReadUInt64LittleEndian(binary);
                    break;
                default:
                    if (type == typeof(TimeValue))
                    {
                        Array.Resize(ref binary, 8);
                        resObject = new TimeValue(BinaryPrimitives.ReadInt64LittleEndian(binary));
                    }
                    else
                        resObject = value.Value;
                    break;
            }

            if (type.IsEnum)
                resObject = Enum.ToObject(type, resObject);


            field.SetValue(instance, resObject);
            return true;
        }

        private bool ReadTdfString(Stream stream, ref object? instance, FieldInfo? field)
        {
            string? str = stream.ReadTdfString();
            if (str == null)
                return false;

            field?.SetValue(instance, str);
            return true;
        }

        private bool ReadTdfBlob(Stream stream, ref object? instance, FieldInfo? field)
        {
            byte[]? blob = stream.ReadTdfBlob();
            if (blob == null)
                return false;

            field?.SetValue(instance, blob);
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


        private bool ReadTdfStruct(Stream stream, ref object? instance, FieldInfo? field)
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

        private bool ReadTdfList(Stream stream, ref object? instance, FieldInfo? field)
        {
            Type? listFullType = field?.FieldType;
            Type? listMemberType = null;
            if (listFullType != null)
            {
                Type[] genericArgTypes = listFullType.GetGenericArguments();
                if (genericArgTypes.Length > 0)
                    listMemberType = genericArgTypes[0];
            }


            TdfBaseType baseType = stream.ReadTdfBaseType();
            ulong? countNullable = (ulong?)stream.ReadTdfInteger();
            if (countNullable == null)
                return false;
            ulong count = countNullable.Value;

            #region bug implementation fix

            if (_heat1Bug && baseType == TdfBaseType.TDF_TYPE_STRUCT && listMemberType != null)
            {
                if (listMemberType.BaseType == typeof(TdfUnion))
                    baseType = TdfBaseType.TDF_TYPE_UNION;
                else
                {
                    if (listMemberType.IsGenericType)
                        listMemberType = listMemberType.GetGenericTypeDefinition();

                    if (listMemberType == typeof(List<>))
                        baseType = TdfBaseType.TDF_TYPE_LIST;
                    else if (listMemberType == typeof(Dictionary<,>) || listMemberType == typeof(SortedDictionary<,>))
                        baseType = TdfBaseType.TDF_TYPE_MAP;
                }

            }
            #endregion

            TdfReader? reader = GetTdfReader(baseType);
            if (reader == null)
                return false;

            //unknown type, skip it
            if (listFullType == null || listMemberType == null)
            {
                object? obj = new object();
                for (ulong i = 0; i < count; i++)
                {
                    if (!reader(stream, ref obj, null))
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
                if (!reader(stream, ref typeContainerObj, typeContainer.ValueFieldInfo))
                    return false;

                addMethod.Invoke(list, new[] { typeContainer.Value });
            }

            field?.SetValue(instance, list);
            return true;
        }


        private bool ReadTdfMap(Stream stream, ref object? instance, FieldInfo? field)
        {
            TdfBaseType keyDataType = stream.ReadTdfBaseType();
            TdfBaseType valueDataType = stream.ReadTdfBaseType();
            ulong? countNullable = (ulong?)stream.ReadTdfInteger();
            if (countNullable == null)
                return false;
            ulong count = countNullable.Value;

            TdfReader? keyReader = GetTdfReader(keyDataType);
            TdfReader? valueReader = GetTdfReader(valueDataType);
            if (keyReader == null || valueReader == null)
                return false;

            Type? mapFullType = field?.FieldType;
            Type? mapKeyType = mapFullType?.GetGenericArguments()[0];
            Type? mapValueType = mapFullType?.GetGenericArguments()[1];

            //unknown type, skip it
            if (mapFullType == null || mapKeyType == null || mapValueType == null)
            {
                object? obj = new object();
                for (ulong i = 0; i < count; i++)
                {
                    if (!keyReader(stream, ref obj, null))
                        return false;
                    if (!valueReader(stream, ref obj, null))
                        return false;
                }
                return true;
            }

            var map = Activator.CreateInstance(mapFullType);
            MethodInfo addMethod = mapFullType.GetMethod("Add")!;

            object? keyContainerObj = Activator.CreateInstance(typeof(TdfValueContainer<>).MakeGenericType(mapKeyType))!;
            ITdfValueContainer keyContainer = (ITdfValueContainer?)keyContainerObj!;

            object? valueContainerObj = Activator.CreateInstance(typeof(TdfValueContainer<>).MakeGenericType(mapValueType))!;
            ITdfValueContainer valueContainer = (ITdfValueContainer?)valueContainerObj!;

            for (ulong i = 0; i < count; i++)
            {
                if (!keyReader(stream, ref keyContainerObj, keyContainer.ValueFieldInfo))
                    return false;
                if (!valueReader(stream, ref valueContainerObj, valueContainer.ValueFieldInfo))
                    return false;

                addMethod.Invoke(map, new[] { keyContainer.Value, valueContainer.Value });
            }

            field?.SetValue(instance, map);
            return true;
        }


        private bool ReadTdfUnion(Stream stream, ref object? instance, FieldInfo? field)
        {
            int activeMember = stream.ReadByte();
            if (activeMember == -1)
                return false;

            byte[] peek = new byte[TdfUnion.TDF_VALU_TAG.Length];
            if (!stream.ReadAll(peek, 0, peek.Length))
                return false;

            //don't care about the valu tag with struct datatype, now we can read this as a struct, this also fixes the bug with the list of unions
            if (!peek.SequenceEqual(TdfUnion.TDF_VALU_TAG))
                stream.Seek(-peek.Length, SeekOrigin.Current);

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

        private bool ReadTdfVariable(Stream stream, ref object? instance, FieldInfo? field)
        {
            int b = stream.ReadByte();
            if (b == -1)
                return false;

            bool present = b != 0;
            if (!present)
            {
                field?.SetValue(instance, null);
                return true;
            }

            uint? tdfId = (uint?)stream.ReadTdfInteger();
            if (tdfId == null)
                return false;

            Type? fieldType = _factory.GetType(tdfId.Value);

            stream.Seek(3, SeekOrigin.Current); //skip the same tag again
            TdfBaseType baseType = stream.ReadTdfBaseType();

            TdfReader? reader = GetTdfReader(baseType);
            if (reader == null)
                return false;

            if (!reader(stream, ref instance, field != null ? new TdfVariableFieldInfo(field, fieldType) : null))
                return false;

            return stream.ReadByte() == 0x00; //tdf variable terminator
        }

        private bool ReadTdfBlazeObjectType(Stream stream, ref object? instance, FieldInfo? field)
        {
            BlazeObjectType? value = stream.ReadTdfBlazeObjectType();
            if (value == null)
                return false;

            field?.SetValue(instance, value.Value);
            return true;
        }

        private bool ReadTdfBlazeObjectId(Stream stream, ref object? instance, FieldInfo? field)
        {
            BlazeObjectId? value = stream.ReadTdfBlazeObjectId();
            if (value == null)
                return false;

            field?.SetValue(instance, value.Value);
            return true;
        }

        private bool ReadTdfFloat(Stream stream, ref object? instance, FieldInfo? field)
        {
            float? value = stream.ReadTdfFloat();
            if (value == null)
                return false;

            field?.SetValue(instance, value.Value);
            return true;
        }

        private bool ReadTdfTimeValue(Stream stream, ref object? instance, FieldInfo? field)
        {
            //This should never happen, since TimeValues are encoded using TDF_TYPE_INTEGER
            return false;
        }


    }
}
