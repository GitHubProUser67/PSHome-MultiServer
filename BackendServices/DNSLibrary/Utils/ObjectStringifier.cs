using System.Collections;
using System.Reflection;
using System.Text;

namespace DNSLibrary.Utils
{
    public class ObjectStringifier
    {
        public static ObjectStringifier New(object obj)
        {
            return new ObjectStringifier(obj);
        }

        public static string Stringify(object obj)
        {
            return StringifyObject(obj);
        }

        private static string StringifyObject(object obj)
        {
            return obj is string v ? v
                : obj is IDictionary dictionary ? StringifyDictionary(dictionary)
                : obj is IEnumerable enumerable ? StringifyList(enumerable)
                : obj == null ? "null"
                : obj.ToString();
        }

        private static string StringifyList(IEnumerable enumerable)
        {
            return "["
                + string.Join(
                    ", ",
                    enumerable.Cast<object>().Select(o => StringifyObject(o)).ToArray()
                )
                + "]";
        }

        private static string StringifyDictionary(IDictionary dict)
        {
            var result = new StringBuilder();

            result.Append('{');

            foreach (DictionaryEntry pair in dict)
            {
                result
                    .Append(pair.Key)
                    .Append('=')
                    .Append(StringifyObject(pair.Value))
                    .Append(", ");
            }

            if (result.Length > 1)
                result.Remove(result.Length - 2, 2);

            return result.Append('}').ToString();
        }

        private readonly object obj;
        private readonly Dictionary<string, string> pairs;
        private static readonly object[] index = Array.Empty<object>();

        public ObjectStringifier(object obj)
        {
            this.obj = obj;
            pairs = new Dictionary<string, string>();
        }

        public ObjectStringifier Remove(params string[] names)
        {
            foreach (var name in names)
                pairs.Remove(name);

            return this;
        }

        public ObjectStringifier Add(params string[] names)
        {
            var type = obj.GetType();

            foreach (var name in names)
            {
                var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                var value = property.GetValue(obj, index);

                pairs.Add(name, StringifyObject(value));
            }

            return this;
        }

        public ObjectStringifier Add(string name, object value)
        {
            pairs.Add(name, StringifyObject(value));
            return this;
        }

        public ObjectStringifier AddAll()
        {
            var properties = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj, index);
                pairs.Add(property.Name, StringifyObject(value));
            }

            return this;
        }

        public override string ToString()
        {
            return StringifyDictionary(pairs);
        }
    }
}
