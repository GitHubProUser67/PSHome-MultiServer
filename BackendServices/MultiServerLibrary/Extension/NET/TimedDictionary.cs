using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class TimedDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _storage = new ConcurrentDictionary<TKey, TValue>();

        public void Set(TKey key, TValue value, int millisecondsExpiration)
        {
            _storage[key] = value;
            _ = ExpireKeyAfterDelay(key, millisecondsExpiration);
        }

        public TValue Get(TKey key)
        {
            _storage.TryGetValue(key, out var value);
            return value;
        }

        private async Task<bool> ExpireKeyAfterDelay(TKey key, int millisecondsExpiration)
        {
            await Task.Delay(millisecondsExpiration).ConfigureAwait(false);
            return _storage.TryRemove(key, out _);
        }

        public bool TryGetValue(TKey key, out TValue value) => _storage.TryGetValue(key, out value);

        public bool ContainsKey(TKey key) => _storage.ContainsKey(key);

        public bool Remove(TKey key) => _storage.TryRemove(key, out _);
    }

}
