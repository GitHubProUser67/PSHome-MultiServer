using System.Collections.Generic;
using System.Linq;

namespace System
{
    public sealed class UniqueIDGenerator
	{
		private readonly object _lock = new object();

		private readonly uint _minId;
		private uint _nextId;

		private readonly HashSet<uint> _activeIds = new HashSet<uint>();
		private readonly HashSet<uint> _freedIds = new HashSet<uint>();

		public UniqueIDGenerator(uint startingValue = 1)
		{
			if (startingValue == 0)
				throw new ArgumentException("[UniqueIDGenerator] - Starting value cannot be 0.");

			_minId = startingValue;
			_nextId = startingValue - 1;
		}

		public uint CreateUniqueID()
		{
			lock (_lock)
			{
				uint limit = uint.MaxValue - _minId + 1;

				// ✅ HARD STOP: prevent endless scan
				if (_activeIds.Count < limit)
				{
					while (_freedIds.Count > 0)
					{
						uint reused = _freedIds.First();
						_freedIds.Remove(reused);
						if (_activeIds.Add(reused))
							return reused;
					}

					for (uint i = 0; i < limit; i++)
					{
						_nextId++;

						if (_nextId == 0 || _nextId < _minId)
							_nextId = _minId;

						if (_activeIds.Add(_nextId))
							return _nextId;
					}
				}
			}

			throw new InvalidOperationException("[UniqueIDGenerator] - No available unique IDs.");
		}

		public uint CreateSequentialID()
		{
			lock (_lock)
				return ++_nextId;
		}

		public bool ReleaseID(uint id)
		{
			if (id < _minId)
				return false;

			lock (_lock)
			{
				if (_activeIds.Remove(id))
					return _freedIds.Add(id);
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
