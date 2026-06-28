namespace Prometheus.HttpClientMetrics;

internal sealed class HttpClientRequestCountHandler(
    HttpClientRequestCountOptions? options,
    HttpClientIdentity identity
)
    : HttpClientDelegatingHandlerBase<ICollector<ICounter>, ICounter>(
        options,
        options?.Counter,
        identity
    )
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage? response = null;

        try
        {
            response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return response;
        }
        finally
        {
            CreateChild(request, response).Inc();
        }
    }

    protected override string[] DefaultLabels => HttpClientRequestLabelNames.All;

    protected override ICollector<ICounter> CreateMetricInstance(string[] labelNames) =>
        MetricFactory.CreateCounter(
            "httpclient_requests_sent_total",
            "Count of HTTP requests that have been completed by an HttpClient.",
            labelNames
        );
}
