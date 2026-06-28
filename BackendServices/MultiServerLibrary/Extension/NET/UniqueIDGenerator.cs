namespace MultiServerLibrary.Extension.NET
{
    public sealed class UniqueIDGenerator
    {
        private readonly Lock _lock = new();

        private readonly uint _minId;
        private readonly uint _maxId;

        private uint _nextId;

        private readonly HashSet<uint> _activeIds = new();
        private readonly Queue<uint> _freedIds = new();

        public UniqueIDGenerator(uint startingValue = 1, uint maxValue = uint.MaxValue)
        {
            if (startingValue == 0)
                throw new ArgumentException(
                    "[UniqueIDGenerator] - Starting value cannot be 0.",
                    nameof(startingValue)
                );

            if (startingValue > maxValue)
                throw new ArgumentException(
                    "[UniqueIDGenerator] - Starting value must be <= max value."
                );

            _minId = startingValue;
            _maxId = maxValue;
            _nextId = startingValue - 1;
        }

        public uint CreateUniqueID()
        {
            lock (_lock)
            {
                if (_freedIds.Count > 0)
                {
                    var reused = _freedIds.Dequeue();
                    _activeIds.Add(reused);
                    return reused;
                }

                var nextId = _nextId + 1;

                if (nextId > _maxId || nextId < _minId)
                    throw new InvalidOperationException(
                        "[UniqueIDGenerator] - No available unique IDs."
                    );

                ++_nextId;

                _activeIds.Add(nextId);
                return nextId;
            }
        }

        // Provided for backward compatibility only, do not use it.
        public uint CreateSequentialID()
        {
            lock (_lock)
                return ++_nextId;
        }

        public bool ReleaseID(uint id)
        {
            lock (_lock)
            {
                if (_activeIds.Remove(id))
                {
                    _freedIds.Enqueue(id);
                    return true;
                }
            }

            return false;
        }

        public bool IsInUse(uint id)
        {
            lock (_lock)
                return _activeIds.Contains(id);
        }

        public int ActiveCount
        {
            get
            {
                lock (_lock)
                    return _activeIds.Count;
            }
        }
    }
}
