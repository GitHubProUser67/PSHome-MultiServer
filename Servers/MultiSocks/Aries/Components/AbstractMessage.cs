using System.Text;
using EndianTools;

namespace MultiSocks.Aries.Components
{
    public abstract class AbstractMessage
    {
        public abstract string _Name { get; }
        public uint ErrorCode { get; set; }
        public Dictionary<string, string?> OutputCache = new();
        private readonly Dictionary<string, string?> InputCache = new();

        public void Read(string input)
        {
            input = input.TrimEnd('\0');

            var hasN = input.IndexOf('\n') > 0;
            var hasSpace = input.IndexOf(' ') > 0;
            var pairs =
                hasN ? input.Split('\n')
                : hasSpace ? input.Split(' ')
                : input.Split((char)9);
            foreach (var pair in pairs)
            {
                if (pair.Length == 0)
                    continue;
                var eqSplit = pair.Split('=');
                if (eqSplit.Length < 2)
                    continue;
                var value = eqSplit[1];
                if (eqSplit.Length > 1)
                {
                    if (string.IsNullOrEmpty(value))
                        continue;
                    else if (value[0] == '\"' && value[^1] == '\"')
                        value = value[1..^1];
                }

                if (InputCache.TryAdd(eqSplit[0], value))
                {
#if DEBUG
                    CustomLogger.LoggerAccessor.LogInfo(
                        $"[AbstractMessage] - {_Name} - Property: {eqSplit[0]} with Value: {value} was added to the content cache!"
                    );
#endif
                }
                else
                    CustomLogger.LoggerAccessor.LogError(
                        $"[AbstractMessage] - {_Name} - Property: {eqSplit[0]} with Value: {value} couldn't be added to the content cache!"
                    );
            }
        }

        public string Write()
        {
            var type = GetType();
            var props = type.GetProperties();
            StringBuilder keyValue = new();
            foreach (var prop in props)
            {
                if (
                    (
                        prop.PropertyType != typeof(string)
                        && prop.PropertyType != typeof(string[])
                        && prop.PropertyType != typeof(Dictionary<string, string>)
                        && prop.PropertyType != typeof(Dictionary<string, string[]>)
                    )
                    || prop.Name[0] == '_'
                )
                    continue;
                if (prop.PropertyType == typeof(string[]))
                {
                    var values = (string[]?)prop.GetValue(this);
                    if (values == null)
                        continue;
                    for (var i = 0; i < values.Length; i++)
                    {
                        if (values[i] != null)
                            keyValue.Append(EncodeKV(prop.Name, values[i]));
                    }
                }
                else if (prop.PropertyType == typeof(Dictionary<string, string>))
                {
                    var values = (Dictionary<string, string>?)prop.GetValue(this);
                    if (values == null)
                        continue;
                    foreach (var dicprop in values)
                    {
                        if (dicprop.Value != null)
                            keyValue.Append(EncodeKV(dicprop.Key, dicprop.Value));
                    }
                }
                else if (prop.PropertyType == typeof(Dictionary<string, string[]>))
                {
                    var values = (Dictionary<string, string[]>?)prop.GetValue(this);
                    if (values == null)
                        continue;
                    foreach (var dicprop in values)
                    {
                        var value = dicprop.Value;
                        if (value != null)
                        {
                            for (var i = 0; i < value.Length; i++)
                            {
                                if (value[i] != null)
                                    keyValue.Append(EncodeKV(dicprop.Key, value[i]));
                            }
                        }
                    }
                }
                else
                {
                    var value = (string?)prop.GetValue(this);
                    if (value == null)
                        continue;
                    keyValue.Append(EncodeKV(prop.Name, value));
                }
            }
            foreach (var prop in OutputCache)
            {
                var value = prop.Value;
                if (value == null)
                    continue;
                keyValue.Append(EncodeKV(prop.Key, value));
            }
            if (keyValue.Length == 0)
                keyValue.Append('\n');

            return keyValue.ToString();
        }

        private static string EncodeKV(string key, string value)
        {
            return key + "=" + value + '\n';
        }

        public virtual void Process(AbstractAriesServer context, AriesClient client) { }

        public byte[] GetData()
        {
            var packetIdentLength = _Name.Length;
            if (packetIdentLength < 4 || packetIdentLength > 8)
                throw new InvalidDataException(
                    $"[AbstractMessage] - Invalide Name:{_Name} choosen for the packet! Please follow the guidelines on the naming convention."
                );

            var hasStatusCode = packetIdentLength == 8;
            var header = new byte[hasStatusCode ? 4 : 8];
            var body = Write() + "\0";
            var size = body.Length + 12;

            MemoryStream mem = new();
            BinaryWriter io = new(mem);
            io.Write(Encoding.ASCII.GetBytes(_Name));
            if (hasStatusCode)
                EndianAwareConverter.WriteUInt32(header, Endianness.BigEndian, 0, (uint)size);
            else
            {
                EndianAwareConverter.WriteUInt32(header, Endianness.BigEndian, 0, ErrorCode);
                EndianAwareConverter.WriteUInt32(header, Endianness.BigEndian, 4, (uint)size);
            }
            io.Write(header);
            io.Write(Encoding.ASCII.GetBytes(body));

            var bytes = mem.ToArray();
            io.Dispose();
            mem.Dispose();
            return bytes;
        }

        public string? GetInputCacheValue(string key)
        {
            return InputCache.TryGetValue(key, out var value) ? value : null;
        }

        public void CopyInputCacheToOutputCache()
        {
            OutputCache = InputCache;
        }
    }
}
