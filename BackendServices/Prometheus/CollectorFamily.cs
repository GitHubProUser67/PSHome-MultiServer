using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.ObjectPool;

namespace Prometheus;

internal sealed class CollectorFamily
{
    public Type CollectorType { get; }

    private readonly Dictionary<CollectorIdentity, Collector> _collectors = [];
    private readonly ReaderWriterLockSlim _lock = new();

    public CollectorFamily(Type collectorType)
    {
        CollectorType = collectorType;
        _collectAndSerializeFunc = CollectAndSerialize;
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    internal async ValueTask CollectAndSerializeAsync(
        IMetricsSerializer serializer,
        CancellationToken cancel
    )
    {
        var operation = _serializeFamilyOperationPool.Get();
        operation.Serializer = serializer;

        await ForEachCollectorAsync(_collectAndSerializeFunc, operation, cancel)
            .ConfigureAwait(false);

        _serializeFamilyOperationPool.Return(operation);
    }

    /// <summary>
    /// We use these reusable operation wrappers to avoid capturing variables when serializing, to keep memory usage down while serializing.
    /// </summary>
    private sealed class SerializeFamilyOperation
    {
        // The first family member we serialize requires different serialization from the others.
        public bool IsFirst;
        public IMetricsSerializer? Serializer;

        public SerializeFamilyOperation() => Reset();

        public void Reset()
        {
            IsFirst = true;
            Serializer = null;
        }
    }

    // We have a bunch of families that get serialized often - no reason to churn the GC with a bunch of allocations if we can easily reuse it.
    private static readonly ObjectPool<SerializeFamilyOperation> _serializeFamilyOperationPool =
        ObjectPool.Create(new SerializeFamilyOperationPoolingPolicy());

    private sealed class SerializeFamilyOperationPoolingPolicy
        : PooledObjectPolicy<SerializeFamilyOperation>
    {
        public override SerializeFamilyOperation Create() => new();

        public override bool Return(SerializeFamilyOperation obj)
        {
            obj.Reset();
            return true;
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask CollectAndSerialize(
        Collector collector,
        SerializeFamilyOperation operation,
        CancellationToken cancel
    )
    {
        await collector
            .CollectAndSerializeAsync(operation.Serializer!, operation.IsFirst, cancel)
            .ConfigureAwait(false);
        operation.IsFirst = false;
    }

    private readonly Func<
        Collector,
        SerializeFamilyOperation,
        CancellationToken,
        ValueTask
    > _collectAndSerializeFunc;

    internal Collector GetOrAdd<TCollector, TConfiguration>(
        in CollectorIdentity identity,
        string name,
        string help,
        TConfiguration configuration,
        ExemplarBehavior exemplarBehavior,
        CollectorRegistry.CollectorInitializer<TCollector, TConfiguration> initializer
    )
        where TCollector : Collector
        where TConfiguration : MetricConfiguration
    {
        // First we try just holding a read lock. This is the happy path.
        _lock.EnterReadLock();

        try
        {
            if (_collectors.TryGetValue(identity, out var collector))
                return collector;
        }
        finally
        {
            _lock.ExitReadLock();
        }

        // Then we grab a write lock. This is the slow path.
        var newCollector = initializer(
            name,
            help,
            identity.InstanceLabelNames,
            identity.StaticLabels,
            configuration,
            exemplarBehavior
        );

        _lock.EnterWriteLock();

        try
        {
            // It could be that someone beats us to it! Probably not, though.
            return _collectors.TryAdd(identity, newCollector)
                ? newCollector
                : _collectors[identity];
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    internal void ForEachCollector(Action<Collector> action)
    {
        _lock.EnterReadLock();

        try
        {
            foreach (var collector in _collectors.Values)
                action(collector);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    internal async ValueTask ForEachCollectorAsync<TArg>(
        Func<Collector, TArg, CancellationToken, ValueTask> func,
        TArg arg,
        CancellationToken cancel
    )
        where TArg : class
    {
        // This could potentially take nontrivial time, as we are serializing to a stream (potentially, a network stream).
        // Therefore we operate on a defensive copy in a reused buffer.
        Collector[] buffer;

        _lock.EnterReadLock();

        var collectorCount = _collectors.Count;
        buffer = ArrayPool<Collector>.Shared.Rent(collectorCount);

        try
        {
            try
            {
                _collectors.Values.CopyTo(buffer, 0);
            }
            finally
            {
                _lock.ExitReadLock();
            }

            for (var i = 0; i < collectorCount; i++)
            {
                var collector = buffer[i];
                await func(collector, arg, cancel).ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<Collector>.Shared.Return(buffer, clearArray: true);
        }
    }
}
