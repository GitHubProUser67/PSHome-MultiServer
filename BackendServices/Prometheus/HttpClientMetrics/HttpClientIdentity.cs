namespace Prometheus.HttpClientMetrics;

public sealed class HttpClientIdentity(string name)
{
    public static readonly HttpClientIdentity Default = new("default");

    public string Name { get; } = name;
}
