namespace Prometheus;

/// <summary>
/// Applies a set of static labels to lifetime-managed metrics. Multiple instances are functionally equivalent for the same label set.
/// </summary>
internal sealed class LabelEnrichingManagedLifetimeMetricFactory(
    ManagedLifetimeMetricFactory inner,
    IDictionary<string, string> enrichWithLabels
) : IManagedLifetimeMetricFactory
{
    private readonly ManagedLifetimeMetricFactory _inner = inner;

    // This is an ordered list because labels have specific order.
    private readonly IReadOnlyList<KeyValuePair<string, string>> _labels = enrichWithLabels
        .OrderBy(x => x.Key, StringComparer.Ordinal)
        .ToList();

    // Cache the names/values to enrich with, for reuse.
    // We could perhaps improve even further via StringSequence but that requires creating separate internal APIs so can be a future optimization.
    private readonly string[] _enrichWithLabelNames = [.. enrichWithLabels.Select(x => x.Key)];
    private readonly string[] _enrichWithLabelValues = [.. enrichWithLabels.Select(x => x.Value)];

    public IManagedLifetimeMetricHandle<ICounter> CreateCounter(
        string name,
        string help,
        string[]? instanceLabelNames,
        CounterConfiguration? configuration
    )
    {
        var combinedLabelNames = WithEnrichedLabelNames(instanceLabelNames ?? []);
        var innerHandle = _inner.CreateCounter(name, help, combinedLabelNames, configuration);

        // 1-1 relationship between instance of inner handle and our labeling handle.
        // We expect lifetime of each to match the lifetime of the respective factory, so no need to cleanup anything.

        _countersLock.EnterReadLock();

        try
        {
            if (_counters.TryGetValue(innerHandle, out var existing))
                return existing;
        }
        finally
        {
            _countersLock.ExitReadLock();
        }

        var instance = CreateCounterCore(innerHandle);

        _countersLock.EnterWriteLock();

        try
        {
            // It could be that someone beats us to it! Probably not, though.
            return _counters.TryAdd(innerHandle, instance) ? instance : _counters[innerHandle];
        }
        finally
        {
            _countersLock.ExitWriteLock();
        }
    }

    private LabelEnrichingManagedLifetimeCounter CreateCounterCore(
        IManagedLifetimeMetricHandle<ICounter> inner
    ) => new(inner, _enrichWithLabelValues);

    private readonly Dictionary<
        IManagedLifetimeMetricHandle<ICounter>,
        LabelEnrichingManagedLifetimeCounter
    > _counters = [];
    private readonly ReaderWriterLockSlim _countersLock = new();

    public IManagedLifetimeMetricHandle<IGauge> CreateGauge(
        string name,
        string help,
        string[]? instanceLabelNames,
        GaugeConfiguration? configuration
    )
    {
        var combinedLabelNames = WithEnrichedLabelNames(instanceLabelNames ?? []);
        var innerHandle = _inner.CreateGauge(name, help, combinedLabelNames, configuration);

        // 1-1 relationship between instance of inner handle and our labeling handle.
        // We expect lifetime of each to match the lifetime of the respective factory, so no need to cleanup anything.

        _gaugesLock.EnterReadLock();

        try
        {
            if (_gauges.TryGetValue(innerHandle, out var existing))
                return existing;
        }
        finally
        {
            _gaugesLock.ExitReadLock();
        }

        var instance = CreateGaugeCore(innerHandle);

        _gaugesLock.EnterWriteLock();

        try
        {
            // It could be that someone beats us to it! Probably not, though.
            return _gauges.TryAdd(innerHandle, instance) ? instance : _gauges[innerHandle];
        }
        finally
        {
            _gaugesLock.ExitWriteLock();
        }
    }

    private LabelEnrichingManagedLifetimeGauge CreateGaugeCore(
        IManagedLifetimeMetricHandle<IGauge> inner
    ) => new(inner, _enrichWithLabelValues);

    private readonly Dictionary<
        IManagedLifetimeMetricHandle<IGauge>,
        LabelEnrichingManagedLifetimeGauge
    > _gauges = [];
    private readonly ReaderWriterLockSlim _gaugesLock = new();

    public IManagedLifetimeMetricHandle<IHistogram> CreateHistogram(
        string name,
        string help,
        string[]? instanceLabelNames,
        HistogramConfiguration? configuration
    )
    {
        var combinedLabelNames = WithEnrichedLabelNames(instanceLabelNames ?? []);
        var innerHandle = _inner.CreateHistogram(name, help, combinedLabelNames, configuration);

        // 1-1 relationship between instance of inner handle and our labeling handle.
        // We expect lifetime of each to match the lifetime of the respective factory, so no need to cleanup anything.

        _histogramsLock.EnterReadLock();

        try
        {
            if (_histograms.TryGetValue(innerHandle, out var existing))
                return existing;
        }
        finally
        {
            _histogramsLock.ExitReadLock();
        }

        var instance = CreateHistogramCore(innerHandle);

        _histogramsLock.EnterWriteLock();

        try
        {
            // It could be that someone beats us to it! Probably not, though.
            return _histograms.TryAdd(innerHandle, instance) ? instance : _histograms[innerHandle];
        }
        finally
        {
            _histogramsLock.ExitWriteLock();
        }
    }

    private LabelEnrichingManagedLifetimeHistogram CreateHistogramCore(
        IManagedLifetimeMetricHandle<IHistogram> inner
    ) => new(inner, _enrichWithLabelValues);

    private readonly Dictionary<
        IManagedLifetimeMetricHandle<IHistogram>,
        LabelEnrichingManagedLifetimeHistogram
    > _histograms = [];
    private readonly ReaderWriterLockSlim _histogramsLock = new();

    public IManagedLifetimeMetricHandle<ISummary> CreateSummary(
        string name,
        string help,
        string[]? instanceLabelNames,
        SummaryConfiguration? configuration
    )
    {
        var combinedLabelNames = WithEnrichedLabelNames(instanceLabelNames ?? []);
        var innerHandle = _inner.CreateSummary(name, help, combinedLabelNames, configuration);

        // 1-1 relationship between instance of inner handle and our labeling handle.
        // We expect lifetime of each to match the lifetime of the respective factory, so no need to cleanup anything.

        _summariesLock.EnterReadLock();

        try
        {
            if (_summaries.TryGetValue(innerHandle, out var existing))
                return existing;
        }
        finally
        {
            _summariesLock.ExitReadLock();
        }

        var instance = CreateSummaryCore(innerHandle);

        _summariesLock.EnterWriteLock();

        try
        {
            // It could be that someone beats us to it! Probably not, though.
            return _summaries.TryAdd(innerHandle, instance) ? instance : _summaries[innerHandle];
        }
        finally
        {
            _summariesLock.ExitWriteLock();
        }
    }

    private LabelEnrichingManagedLifetimeSummary CreateSummaryCore(
        IManagedLifetimeMetricHandle<ISummary> inner
    ) => new(inner, _enrichWithLabelValues);

    private readonly Dictionary<
        IManagedLifetimeMetricHandle<ISummary>,
        LabelEnrichingManagedLifetimeSummary
    > _summaries = [];
    private readonly ReaderWriterLockSlim _summariesLock = new();

    public IManagedLifetimeMetricFactory WithLabels(IDictionary<string, string> labels)
    {
        var combinedLabels = _labels.Concat(labels).ToDictionary(x => x.Key, x => x.Value);

        // Inner factory takes care of applying the correct ordering for labels.
        return _inner.WithLabels(combinedLabels);
    }

    private string[] WithEnrichedLabelNames(string[] instanceLabelNames)
    {
        // Enrichment labels always go first when we are communicating with the inner factory.
        return [.. _enrichWithLabelNames, .. instanceLabelNames];
    }
}
