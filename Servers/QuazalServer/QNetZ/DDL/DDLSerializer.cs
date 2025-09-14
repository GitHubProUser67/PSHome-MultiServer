using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.QNetZ.DDL
{
    public class DDLSerializer
	{
        // Function to get property values
        public static object[] ReadPropertyValues(Type[] typeList, Stream str)
        {
            var paramsInstances = new List<object>();

            foreach (Type? type in typeList)
            {
                if (type == null)
				{
                    paramsInstances.Add(null);
                    continue;
                }

                paramsInstances.Add(ReadObject(type, str));
            }

            return paramsInstances.ToArray();
        }

        // Complex objects

        // reads type and returns new object instance
        public static T? ReadObject<T>(Stream s) where T : class
		{
            return (T?)ReadObject(typeof(T), s);
		}

        public static object? ReadObject(Type currentType, Stream str)
		{
            object? instance = null;

            // handle types
            if (currentType == typeof(string))
                instance = Helper.ReadString(str);
            else if (currentType == typeof(bool))
                instance = Helper.ReadBool(str);
            else if (currentType == typeof(float))
                instance = Helper.ReadFloat(str);
            else if (currentType == typeof(double))
                instance = Helper.ReadDouble(str);
            else if (currentType == typeof(DateTime))
                instance = Helper.ReadDateTime(str);
            else if (currentType == typeof(long))
                instance = (long)Helper.ReadU64(str);
            else if (currentType == typeof(ulong))
                instance = Helper.ReadU64(str);
            else if (currentType == typeof(sbyte))
                instance = (sbyte)Helper.ReadU8(str);
            else if (currentType == typeof(byte))
                instance = Helper.ReadU8(str);
            else if (currentType == typeof(int))
                instance = (int)Helper.ReadU32(str);
            else if (currentType == typeof(uint))
                instance = Helper.ReadU32(str);
            else if (currentType == typeof(short))
                instance = (short)Helper.ReadU16(str);
            else if (currentType == typeof(ushort))
                instance = Helper.ReadU16(str);
            else if (currentType == typeof(byte[])) // This is Quazal.Buffer with 32 bit size
            {
                // byte arrays are special
                uint arrayLen = Helper.ReadU32(str);
                var array = new byte[arrayLen];
                str.Read(array, 0, array.Length);

                instance = array;
            }
			else if (typeof(IAnyData).IsAssignableFrom(currentType))
			{
				var newObjectFunc = Expression.Lambda<Func<IAnyData>>(
					Expression.New(currentType.GetConstructor(Type.EmptyTypes))
				).Compile();

				// emit new AnyData and read it
				var anyObject = newObjectFunc();

				anyObject.Read(str);

				instance = anyObject;
			}
			else if (typeof(IDictionary).IsAssignableFrom(currentType))
            {
                Type[]? dictTypes = currentType.GetGenericArguments();
                Type? dictGenericType = typeof(Dictionary<,>).MakeGenericType(dictTypes);

                // make creation lambda and use default constructor
                var newObjectFunc = Expression.Lambda<Func<IDictionary>>(
                    Expression.New(dictGenericType.GetConstructor(Type.EmptyTypes))
                ).Compile();

                var dictionary = newObjectFunc();
                var size = Helper.ReadU32(str);

                for (int i = 0; i < size; i++)
                {
                    var key = ReadObject(dictTypes[0], str);
                    var value = ReadObject(dictTypes[1], str);

                    dictionary.Add(key, value);
                }
                instance = dictionary;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(currentType))
            {
                Type? arrayItemType = currentType.GetGenericArguments().SingleOrDefault();
                if (arrayItemType != null)
                {
                    var listType = typeof(List<>).MakeGenericType(arrayItemType);

                    // get array size
                    uint size = Helper.ReadU32(str);

                    var newObjectFunc = Expression.Lambda<Func<IList>>(
                        Expression.New(listType.GetConstructor(Type.EmptyTypes))
                    ).Compile();

                    var arrayValues = newObjectFunc();

                    // new array of objects 
                    for (int i = 0; i < size; i++)
                    {
                        var itemInstance = ReadObject(arrayItemType, str);
                        arrayValues.Add(itemInstance);
                    }
                    instance = arrayValues;
                }
            }
            else if (typeof(Stream).IsAssignableFrom(currentType))
                instance = str;
            else if (typeof(RMCPRequest).IsAssignableFrom(currentType))
                instance = Activator.CreateInstance(currentType, new object[] { str });
            else // read complex object
            {
				// collect all properties even from base types in right order
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;
				var allProperties = new List<PropertyInfo>();

                var nType = currentType;
				int n = 0;
                do
                {
					allProperties.InsertRange(0, nType.GetProperties(bindingFlags));
					nType = nType.BaseType;
					n++;
                } while (nType != null);

                // make creation lambda and use default constructor
                var newObjectFunc = Expression.Lambda<Func<object>>(
                    Expression.New(currentType.GetConstructor(Type.EmptyTypes))
                ).Compile();

                // get types and skip read-only
                var allPropertyTypes = allProperties.Select(x => x.CanWrite ? x.PropertyType : null);

                if (allPropertyTypes != null)
                {
                    // this will recurse
                    var allPropertyValues = ReadPropertyValues(allPropertyTypes.ToArray(), str);

                    // create instance
                    instance = newObjectFunc();

                    // assign all values to new instance
                    for (int i = 0; i < allProperties.Count; i++)
                    {
                        allProperties[i].SetValue(instance, allPropertyValues[i]);
                    }

                    return instance;
                }
            }

            return instance;
		}

        // writes object to buffer
		public static void WriteObject<T>(T obj, Stream s) where T : class
		{
            WriteObject(typeof(T), obj, s);
        }

        public static void WriteObject(Type currentType, object obj, Stream str)
        {
            // handle types
            if (currentType == typeof(string))
                Helper.WriteString(str, (string)Convert.ChangeType(obj, currentType));
            else if (currentType == typeof(bool))
                Helper.WriteBool(str, (bool)Convert.ChangeType(obj, currentType));
            else if (currentType == typeof(float))
                Helper.WriteFloat(str, (float)Convert.ChangeType(obj, currentType));
            else if (currentType == typeof(double))
                Helper.WriteDouble(str, (double)Convert.ChangeType(obj, currentType));
            else if (currentType == typeof(DateTime))
                Helper.WriteDateTime(str, (DateTime)Convert.ChangeType(obj, currentType));
            else if (currentType == typeof(ulong)) // Bloody hell...
                Helper.WriteU64(str, (ulong)obj);
            else if (currentType == typeof(long))
                Helper.WriteU64(str, (ulong)(long)obj);
            else if (currentType == typeof(byte))
                Helper.WriteU8(str, (byte)obj);
            else if (currentType == typeof(sbyte))
                Helper.WriteU8(str, (byte)(sbyte)obj);
            else if (currentType == typeof(uint))
                Helper.WriteU32(str, (uint)obj);
            else if (currentType == typeof(int))
                Helper.WriteU32(str, (uint)(int)obj);
            else if (currentType == typeof(ushort))
                Helper.WriteU16(str, (ushort)obj);
            else if (currentType == typeof(short))
                Helper.WriteU16(str, (ushort)(short)obj);
            else if (currentType == typeof(byte[])) // This is Quazal.Buffer with 32 bit size
            {
                byte[] array = (byte[])obj;

                // byte arrays are special
                Helper.WriteU32(str, (uint)array.Length);
                str.Write(array, 0, array.Length);
            }
			else if (typeof(IAnyData).IsAssignableFrom(currentType))
			{
				// emit new AnyData and read it
				IAnyData anyObject = (IAnyData)obj;
				anyObject.Write(str);
			}
			else if (typeof(IDictionary).IsAssignableFrom(currentType))
			{
                IDictionary dictionary = (IDictionary)obj;
                int size = dictionary.Keys.Count;

                Type[] dictTypes = dictionary.GetType().GetGenericArguments();

                Helper.WriteU32(str, (uint)size);

                foreach (DictionaryEntry entry in dictionary)
				{
                    // write key
                    WriteObject(dictTypes[0], entry.Key, str);

                    if (entry.Value != null)
                        // write value
                        WriteObject(dictTypes[1], entry.Value, str);
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(currentType))
            {
                var arrayValues = (IEnumerable)obj;

                Type? arrayItemType = currentType.GetGenericArguments().SingleOrDefault();

                // store array size
                Helper.WriteU32(str, (uint)arrayValues.Cast<object>().Count());

                // write items
                foreach (var item in arrayValues)
                {
                    if (arrayItemType != null)
                        WriteObject(arrayItemType, item, str);
                }
            }
			else
			{
				// collect all properties even from base types in right order
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;
				var allProperties = new List<PropertyInfo>();

				Type? nType = currentType;
				int n = 0;
				do
				{
					allProperties.InsertRange(0, nType.GetProperties(bindingFlags));
					nType = nType.BaseType;
					n++;
				} while (nType != null);

				// get types and skip read-only
				var allPropertyTypes = allProperties.Select(x => x.CanRead ? x.PropertyType : null);

                // assign all values to new instance
                for (int i = 0; i < allProperties.Count; i++)
                {
                    object? value = allProperties[i].GetValue(obj);
                    if (value != null)
                        WriteObject(allProperties[i].PropertyType, value, str);
                }
            }
        }

		public static string ObjectToString<T>(T obj) where T: class
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented);
		}

		public static string ObjectToString(object obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.Indented);
		}
	}
}
