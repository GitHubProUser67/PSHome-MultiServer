using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MultiServerLibrary.Extension
{
    public static class DictionaryUtils
    {
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(
        this Dictionary<TKey, TValue> source)
        {
            return new ConcurrentDictionary<TKey, TValue>(source);
        }
    }
}
