namespace Prometheus;

public sealed class SuppressDefaultMetricOptions
{
    internal static readonly SuppressDefaultMetricOptions SuppressAll = new()
    {
        SuppressProcessMetrics = true,
        SuppressDebugMetrics = true,
        SuppressEventCounters = true,

        SuppressMeters = true,
    };

    internal static readonly SuppressDefaultMetricOptions SuppressNone = new()
    {
        SuppressProcessMetrics = false,
        SuppressDebugMetrics = false,
        SuppressEventCounters = false,

        SuppressMeters = false,
    };

    /// <summary>
    /// Suppress the current-process-inspecting metrics (uptime, resource use, ...).
    /// </summary>
    public bool SuppressProcessMetrics { get; set; }

    /// <summary>
    /// Suppress metrics that prometheus-net uses to report debug information about itself (e.g. number of metrics exported).
    /// </summary>
    public bool SuppressDebugMetrics { get; set; }

    /// <summary>
    /// Suppress the default .NET Event Counter integration.
    /// </summary>
    public bool SuppressEventCounters { get; set; }

    /// <summary>
    /// Suppress the .NET Meter API integration.
    /// </summary>
    public bool SuppressMeters { get; set; }

    internal sealed class ConfigurationCallbacks
    {
        public Action<EventCounterAdapterOptions> ConfigureEventCounterAdapter = delegate { };

        public Action<MeterAdapterOptions> ConfigureMeterAdapter = delegate { };
    }

    /// <summary>
    /// Configures the default metrics registry based on the requested defaults behavior.
    /// </summary>
    internal void ApplyToDefaultRegistry(ConfigurationCallbacks configurationCallbacks)
    {
        if (!SuppressProcessMetrics)
            DotNetStats.RegisterDefault();

        if (!SuppressDebugMetrics)
            Metrics.DefaultRegistry.StartCollectingRegistryMetrics();

        if (!SuppressEventCounters)
        {
            var options = new EventCounterAdapterOptions();
            configurationCallbacks.ConfigureEventCounterAdapter(options);
            EventCounterAdapter.StartListening(options);
        }

        if (!SuppressMeters)
        {
            var options = new MeterAdapterOptions();
            configurationCallbacks.ConfigureMeterAdapter(options);
            MeterAdapter.StartListening(options);
        }
    }
}
