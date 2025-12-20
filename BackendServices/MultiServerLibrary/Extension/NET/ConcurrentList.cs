using System.Linq;
using System.Threading;

namespace System.Collections.Generic
{
    // https://gist.github.com/ronnieoverby/11c8b6b067872db719d7
    public class ConcurrentList<T> : IList<T>, IDisposable
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private int _size = 0;

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _size;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public int InternalArrayLength
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Length;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        private T[] _items;

        public ConcurrentList(int initialCapacity)
        {
            _items = new T[initialCapacity];
        }

        public ConcurrentList() : this(4)
        { }

        public ConcurrentList(IEnumerable<T> items)
        {
            _items = items.ToArray();
            _size = _items.Length;
        }

        public void Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                int newCount = _size + 1;
                EnsureCapacity(newCount);
                _items[_size] = item;
                _size = newCount;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            _lock.EnterWriteLock();
            try
            {
                T[] arr = items as T[] ?? items.ToArray();
                int newCount = _size + arr.Length;
                EnsureCapacity(newCount);
                Array.Copy(arr, 0, _items, _size, arr.Length);
                _size = newCount;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void EnsureCapacity(int capacity)
        {
            if (_items.Length >= capacity)
                return;

            int doubled;
            checked
            {
                try
                {
                    doubled = _items.Length * 2;
                }
                catch (OverflowException)
                {
                    doubled = int.MaxValue;
                }
            }

            int newLength = Math.Max(doubled, capacity);
            Array.Resize(ref _items, newLength);
        }

        public bool Remove(T item)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                int i = IndexOfInternal(item);

                if (i == -1)
                    return false;

                _lock.EnterWriteLock();
                try
                {
                    RemoveAtInternal(i);
                    return true;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                for (int i = 0; i < _size; i++)
                    // deadlocking potential mitigated by lock recursion enforcement
                    yield return _items[i];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return IndexOfInternal(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private int IndexOfInternal(T item)
        {
            return Array.FindIndex(_items, 0, _size, x => x.Equals(item));
        }

        public void Insert(int index, T item)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                if (index > _size)
                    throw new ArgumentOutOfRangeException("index");

                _lock.EnterWriteLock();
                try
                {
                    int newCount = _size + 1;
                    EnsureCapacity(newCount);

                    // shift everything right by one, starting at index
                    Array.Copy(_items, index, _items, index + 1, _size - index);

                    // insert
                    _items[index] = item;
                    _size = newCount;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }


        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException("match");

            _lock.EnterUpgradeableReadLock();
            try
            {
                _lock.EnterWriteLock();
                try
                {
                    int freeIndex = 0;   // the first free slot in items array

                    // Find the first item which needs to be removed.
                    while (freeIndex < _size && !match(_items[freeIndex])) freeIndex++;
                    if (freeIndex >= _size) return 0;

                    int current = freeIndex + 1;
                    while (current < _size)
                    {
                        // Find the first item which needs to be kept.
                        while (current < _size && match(_items[current])) current++;

                        if (current < _size)
                        {
                            // copy item to the free slot.
                            _items[freeIndex++] = _items[current++];
                        }
                    }

                    Array.Clear(_items, freeIndex, _size - freeIndex);
                    int result = _size - freeIndex;
                    _size = freeIndex;
                    return result;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public void RemoveAt(int index)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                if (index >= _size)
                    throw new ArgumentOutOfRangeException("index");

                _lock.EnterWriteLock();
                try
                {
                    RemoveAtInternal(index);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        private void RemoveAtInternal(int index)
        {
            Array.Copy(_items, index + 1, _items, index, _size - index - 1);
            _size--;

            // release last element
            Array.Clear(_items, _size, 1);
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T Find(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException("match");

            _lock.EnterUpgradeableReadLock();
            try
            {
                for (int i = 0; i < _size; i++)
                {
                    if (match(_items[i]))
                        return _items[i];
                }
                return default;
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return IndexOfInternal(item) != -1;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.EnterReadLock();
            try
            {
                if (_size > array.Length - arrayIndex)
                    throw new ArgumentException("Destination array was not long enough.");

                Array.Copy(_items, 0, array, arrayIndex, _size);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    if (index >= _size)
                        throw new ArgumentOutOfRangeException("index");

                    return _items[index];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterUpgradeableReadLock();
                try
                {

                    if (index >= _size)
                        throw new ArgumentOutOfRangeException("index");

                    _lock.EnterWriteLock();
                    try
                    {
                        _items[index] = value;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
                finally
                {
                    _lock.ExitUpgradeableReadLock();
                }

            }
        }

        public void DoSync(Action<ConcurrentList<T>> action)
        {
            GetSync(l =>
            {
                action(l);
                return 0;
            });
        }

        public TResult GetSync<TResult>(Func<ConcurrentList<T>, TResult> func)
        {
            _lock.EnterWriteLock();
            try
            {
                return func(this);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            _lock.Dispose();
        }
    }
}
