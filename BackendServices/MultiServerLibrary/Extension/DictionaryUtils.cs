using System.Collections.Concurrent;

namespace MultiServerLibrary.Extension
{
    public static class DictionaryUtils
    {
        extension(Dictionary<string, string> headers)
        {
            public string ToHttpHeaders()
            {
                return string.Join(
                    "\r\n",
                    headers.Select(x => string.Format("{0}: {1}", x.Key, x.Value))
                );
            }
        }

        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(
            this Dictionary<TKey, TValue> source
        )
        {
            return new ConcurrentDictionary<TKey, TValue>(source);
        }
    }
}
