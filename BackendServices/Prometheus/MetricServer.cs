using CustomLogger;
using SpaceWizards.HttpListener;
using SpaceWizards.HttpListener.CustomServers;

namespace Prometheus;

/// <summary>
/// Implementation of a Prometheus exporter that serves metrics using HttpListener.
/// This is a stand-alone exporter for apps that do not already have an HTTP server included.
/// </summary>
public class MetricServer : MetricHandler
{
    private readonly Dictionary<ushort, bool> _portConfig;
    private readonly int _maxConcurrentListeners;

    private readonly HTTPServer _Server = new() { PreferHttpSys = false };

    /// <summary>
    /// Only requests that match this predicate will be served by the metric server. This allows you to add authorization checks.
    /// By default (if null), all requests are served.
    /// </summary>
    public Func<HttpListenerRequest, bool>? RequestPredicate { get; set; }

    public MetricServer(
        int port,
        int MaxConcurrentListeners = 10,
        string url = "metrics/",
        CollectorRegistry? registry = null,
        bool useHttps = false
    )
        : this("+", port, MaxConcurrentListeners, url, registry, useHttps) { }

    public MetricServer(
        string hostname,
        int port,
        int MaxConcurrentListeners,
        string url = "metrics/",
        CollectorRegistry? registry = null,
        bool useHttps = false
    )
    {
        var s = useHttps ? "s" : string.Empty;
        _portConfig = new() { { (ushort)port, useHttps } };
        _maxConcurrentListeners = MaxConcurrentListeners;

        _Server.Prefix = $"http{s}://{hostname}:{port}/{url}";

        _registry = registry ?? Metrics.DefaultRegistry;
    }

    private readonly CollectorRegistry _registry;

    protected override Task StartServer(CancellationToken cancel)
    {
        return _Server.StartAsync(
            _portConfig,
            _maxConcurrentListeners,
            null!,
            null!,
            null!,
            async (serverPort, listenerCtx, remoteEP) =>
            {
                var context = (HttpListenerContext)listenerCtx;
                var request = context.Request;
                var response = context.Response;

                try
                {
                    var predicate = RequestPredicate;

                    if (predicate != null && !predicate(request))
                    {
                        // Request rejected by predicate.
                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                        return;
                    }

                    try
                    {
                        // We first touch the response.OutputStream only in the callback because touching
                        // it means we can no longer send headers (the status code).
                        var serializer = new TextSerializer(
                            delegate
                            {
                                response.ContentType =
                                    PrometheusConstants.TextContentTypeWithVersionAndEncoding;
                                response.StatusCode = 200;
                                return response.OutputStream;
                            }
                        );

                        await _registry
                            .CollectAndSerializeAsync(serializer, cancel)
                            .ConfigureAwait(false);
                        response.OutputStream.Dispose();
                    }
                    catch (ScrapeFailedException ex)
                    {
                        // This can only happen before anything is written to the stream, so it
                        // should still be safe to update the status code and report an error.
                        response.StatusCode = 503;

                        if (!string.IsNullOrWhiteSpace(ex.Message))
                        {
                            using (var writer = new StreamWriter(response.OutputStream))
                                writer.Write(ex.Message);
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    if (!_Server.IsAnyListening())
                        return; // We were shut down.

                    LoggerAccessor.LogError(
                        string.Format(
                            "[MetricServer] - Error in {0}: {1}",
                            nameof(MetricServer),
                            ex
                        )
                    );

                    try
                    {
                        response.StatusCode = 500;
                    }
                    catch
                    {
                        // Might be too late in request processing to set response code, so just ignore.
                    }
                }
                finally
                {
                    response.Close();
                }
            },
            cancel
        );
    }
}
