using System.Reflection;
using System.Text;

namespace MultiSocks.Aries.Messages
{
    public abstract class AbstractMessage
    {
        public abstract string _Name { get; }
        public string PlaintextData = string.Empty;
        public uint ErrorCode { get; set; }
        public Dictionary<string, string?> OutputCache = new();
        private Dictionary<string, string?> InputCache = new();

        public void Read(string input)
        {
            input = input.TrimEnd('\0');

            bool hasN = input.IndexOf('\n') > 0;
            bool hasSpace = input.IndexOf(' ') > 0;
            string[] pairs;
            if (hasN) pairs = input.Split('\n');
            else if (hasSpace) pairs = input.Split(' ');
            else pairs = input.Split((char)9);

            foreach (string pair in pairs)
            {
                if (pair.Length == 0) continue;
                string[] eqSplit = pair.Split('=');
                if (eqSplit.Length < 2) continue;
                string value = eqSplit[1];
                if (eqSplit.Length > 1)
                {
                    if (string.IsNullOrEmpty(value)) continue;
                    else if (value[0] == '\"' && value[^1] == '\"')
                        value = value[1..^1];
                }

                if (InputCache.TryAdd(eqSplit[0], value))
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogInfo($"[AbstractMessage] - {_Name} - Property: {eqSplit[0]} with Value: {value} was added to the content cache!");
#endif
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"[AbstractMessage] - {_Name} - Property: {eqSplit[0]} with Value: {value} couldn't be added to the content cache!");
            }
        }

        public string Write()
        {
            Type? type = GetType();
            PropertyInfo[] props = type.GetProperties();
            StringBuilder keyValue = new();
            foreach (PropertyInfo? prop in props)
            {
                if (prop.PropertyType != typeof(string) && prop.PropertyType != typeof(string[]) && prop.PropertyType != typeof(Dictionary<string, string>) && prop.PropertyType != typeof(Dictionary<string, string[]>) || prop.Name[0] == '_') continue;
                if (prop.PropertyType == typeof(string[]))
                {
                    string[]? values = (string[]?)prop.GetValue(this);
                    if (values == null) continue;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] != null)
                            keyValue.Append(EncodeKV(prop.Name, values[i]));
                    }
                }
                else if (prop.PropertyType == typeof(Dictionary<string, string>))
                {
                    Dictionary<string, string>? values = (Dictionary<string, string>?)prop.GetValue(this);
                    if (values == null) continue;
                    foreach (var dicprop in values)
                    {
                        if (dicprop.Value != null)
                            keyValue.Append(EncodeKV(dicprop.Key, dicprop.Value));
                    }
                }
                else if (prop.PropertyType == typeof(Dictionary<string, string[]>))
                {
                    Dictionary<string, string[]>? values = (Dictionary<string, string[]>?)prop.GetValue(this);
                    if (values == null) continue;
                    foreach (var dicprop in values)
                    {
                        string[]? value = dicprop.Value;
                        if (value != null)
                        {
                            for (int i = 0; i < value.Length; i++)
                            {
                                if (value[i] != null)
                                    keyValue.Append(EncodeKV(dicprop.Key, value[i]));
                            }
                        }
                    }
                }
                else
                {
                    string? value = (string?)prop.GetValue(this);
                    if (value == null)
                        continue;
                    keyValue.Append(EncodeKV(prop.Name, value));
                }
            }
            foreach (KeyValuePair<string, string?> prop in OutputCache)
            {
                string? value = prop.Value;
                if (value == null)
                    continue;
                keyValue.Append(EncodeKV(prop.Key, value));
            }
            if (keyValue.Length == 0) keyValue.Append('\n');

            return keyValue.ToString();
        }

        private string EncodeKV(string key, string value)
        {
            return key + "=" + value + '\n';
        }

        public virtual void Process(AbstractAriesServer context, AriesClient client)
        {

        }

        public byte[] GetData()
        {
            bool plaintext = _Name.Length == 8;
            if (plaintext && string.IsNullOrEmpty(PlaintextData))
                PlaintextData = Write();
            string body = (plaintext ? PlaintextData : Write()) + "\0";
            int size = body.Length + 12;

            MemoryStream mem = new();
            BinaryWriter io = new(mem);
            io.Write(Encoding.ASCII.GetBytes(_Name));
            io.Write(new byte[] { (byte)(ErrorCode >> 24), (byte)(ErrorCode >> 16), (byte)(ErrorCode >> 8), (byte)ErrorCode });
            io.Write(new byte[] { (byte)(size >> 24), (byte)(size >> 16), (byte)(size >> 8), (byte)size });
            io.Write(Encoding.ASCII.GetBytes(body));

            byte[] bytes = mem.ToArray();
            io.Dispose();
            mem.Dispose();
            return bytes;
        }

        public string? GetInputCacheValue(string key)
        {
            if (InputCache.ContainsKey(key)) return InputCache[key];
            return null;
        }

        public void CopyInputCacheToOutputCache()
        {
            OutputCache = InputCache;
        }
    }
}
