using System.Net;
using ApacheNet.Models;
using CustomLogger;

namespace ApacheNet.BuildIn.RouteHandlers
{
    public class HeatmapRoute
    {
        /// <summary>
        /// Heatmap API routes for serving client geolocation heatmaps.
        /// </summary>
        public static List<Route> index =
        [
            // GET /heatmap - Returns heatmap as PNG image
            new()
            {
                Name = "Heatmap PNG Image",
                UrlRegex = "^/heatmap/?$",
                Method = "GET",
                Hosts = null,
                Callable = (ctx) =>
                {
                    try
                    {
                        byte[] heatmapBytes = ApacheNetHeatmapTracker
                            .GetCurrentHeatmapAsync()
                            .Result;

                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "image/png";
                        ctx.Response.Headers.Add("Cache-Control", "max-age=60");
                        ctx.Response.Send(heatmapBytes).Wait();
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[HeatmapRoute] Served heatmap ({heatmapBytes.Length} bytes) to {ctx.Request.Source.IpAddress}"
                        );
#endif
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(
                            $"[HeatmapRoute] Error serving heatmap: {ex.Message}"
                        );
                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        ctx.Response.Send("Error generating heatmap").Wait();
                        return true;
                    }
                },
            },
            // GET /heatmap.json - Returns heatmap metadata as JSON
            new()
            {
                Name = "Heatmap JSON Metadata",
                UrlRegex = "^/heatmap\\.json/?$",
                Method = "GET",
                Hosts = null,
                Callable = (ctx) =>
                {
                    try
                    {
                        int trackedCount = ApacheNetHeatmapTracker.GetTrackedIPCount();

                        var response = new
                        {
                            status = "success",
                            trackedClients = trackedCount,
                            heatmapUrl = "/heatmap",
                            embedUrl = "/heatmap/embed",
                            generatedAt = DateTime.UtcNow,
                        };

                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "application/json";
                        ctx.Response.Send(
                                Newtonsoft.Json.JsonConvert.SerializeObject(
                                    response,
                                    Newtonsoft.Json.Formatting.Indented
                                )
                            )
                            .Wait();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[HeatmapRoute] Error serving JSON: {ex.Message}");
                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        ctx.Response.ContentType = "application/json";
                        ctx.Response.Send(
                                Newtonsoft.Json.JsonConvert.SerializeObject(
                                    new { error = "Failed to generate metadata" }
                                )
                            )
                            .Wait();
                        return true;
                    }
                },
            },
            // GET /heatmap/embed - Returns HTML page with embedded heatmap
            new()
            {
                Name = "Heatmap HTML Embed",
                UrlRegex = "^/heatmap/embed/?$",
                Method = "GET",
                Hosts = null,
                Callable = (ctx) =>
                {
                    try
                    {
                        string htmlContent =
                            $@"
                            <!DOCTYPE html>
                            <html lang=""en"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Global Client Heatmap - ApacheNet</title>
                                <style>
                                    body {{
                                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                        background: linear-gradient(135deg, #1e1e1e 0%, #2d2d2d 100%);
                                        margin: 0;
                                        padding: 20px;
                                        color: #ffffff;
                                    }}
                                    .container {{
                                        max-width: 1400px;
                                        margin: 0 auto;
                                    }}
                                    h1 {{
                                        text-align: center;
                                        color: #ff9800;
                                        margin-bottom: 10px;
                                        text-shadow: 2px 2px 4px rgba(0,0,0,0.5);
                                    }}
                                    .subtitle {{
                                        text-align: center;
                                        color: #bbb;
                                        margin-bottom: 30px;
                                    }}
                                    .heatmap-container {{
                                        background: #1a1a1a;
                                        border: 2px solid #ff9800;
                                        border-radius: 8px;
                                        padding: 10px;
                                        box-shadow: 0 4px 15px rgba(255, 152, 0, 0.2);
                                        margin-bottom: 30px;
                                        overflow: hidden;
                                    }}
                                    .heatmap-container img {{
                                        width: 100%;
                                        height: auto;
                                        display: block;
                                        border-radius: 4px;
                                    }}
                                    .stats-grid {{
                                        display: grid;
                                        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
                                        gap: 20px;
                                        margin-bottom: 30px;
                                    }}
                                    .stat-card {{
                                        background: #242424;
                                        border-left: 4px solid #ff9800;
                                        padding: 20px;
                                        border-radius: 8px;
                                        box-shadow: 0 2px 8px rgba(0,0,0,0.3);
                                    }}
                                    .stat-card h3 {{
                                        margin: 0 0 10px 0;
                                        color: #ff9800;
                                        font-size: 14px;
                                        text-transform: uppercase;
                                    }}
                                    .stat-card .value {{
                                        font-size: 28px;
                                        font-weight: bold;
                                        color: #fff;
                                    }}
                                    .controls {{
                                        text-align: center;
                                        margin: 20px 0;
                                    }}
                                    button {{
                                        background: #ff9800;
                                        color: #000;
                                        border: none;
                                        padding: 12px 30px;
                                        border-radius: 4px;
                                        font-weight: bold;
                                        cursor: pointer;
                                        margin: 0 10px;
                                        transition: background 0.3s;
                                    }}
                                    button:hover {{
                                        background: #ffa726;
                                    }}
                                    .legend {{
                                        background: #242424;
                                        padding: 20px;
                                        border-radius: 8px;
                                        margin-top: 20px;
                                        display: grid;
                                        grid-template-columns: repeat(4, 1fr);
                                        gap: 15px;
                                    }}
                                    .legend-item {{
                                        display: flex;
                                        align-items: center;
                                        gap: 10px;
                                    }}
                                    .legend-color {{
                                        width: 30px;
                                        height: 30px;
                                        border-radius: 4px;
                                    }}
                                    .low {{ background: #3d2817; }}
                                    .medium {{ background: #ff9800; }}
                                    .high {{ background: #ffb74d; }}
                                    .very-high {{ background: #ffeb99; }}
                                    .footer {{
                                        text-align: center;
                                        color: #888;
                                        margin-top: 30px;
                                        font-size: 12px;
                                    }}
                                    .error {{
                                        background: #d32f2f;
                                        padding: 15px;
                                        border-radius: 4px;
                                        margin-bottom: 20px;
                                        text-align: center;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class=""container"">
                                    <h1>🌍 Global Client Distribution Heatmap</h1>
                                    <div class=""subtitle"">Real-time visualization of where your clients are connecting from</div>

                                    <div class=""stats-grid"">
                                        <div class=""stat-card"">
                                            <h3>Tracked Clients</h3>
                                            <div class=""value"">{ApacheNetHeatmapTracker.GetTrackedIPCount()}</div>
                                        </div>
                                        <div class=""stat-card"">
                                            <h3>Last Updated</h3>
                                            <div class=""value"">{DateTime.Now:HH:mm:ss}</div>
                                        </div>
                                        <div class=""stat-card"">
                                            <h3>Heatmap Resolution</h3>
                                            <div class=""value"">1600x800</div>
                                        </div>
                                        <div class=""stat-card"">
                                            <h3>Update Interval</h3>
                                            <div class=""value"">60 seconds</div>
                                        </div>
                                    </div>

                                    <div class=""heatmap-container"">
                                        <img id=""heatmapImage"" src=""/heatmap?t=${{Date.now()}}"" alt=""Client Distribution Heatmap"" />
                                    </div>

                                    <div class=""controls"">
                                        <button onclick=""refreshHeatmap()"">🔄 Refresh Now</button>
                                        <button onclick=""downloadHeatmap()"">⬇️ Download PNG</button>
                                        <button onclick=""viewStats()"">📊 View Stats</button>
                                    </div>

                                    <div class=""legend"">
                                        <div class=""legend-item"">
                                            <div class=""legend-color low""></div>
                                            <span>Very Low (0-10%)</span>
                                        </div>
                                        <div class=""legend-item"">
                                            <div class=""legend-color medium""></div>
                                            <span>Medium (10-50%)</span>
                                        </div>
                                        <div class=""legend-item"">
                                            <div class=""legend-color high""></div>
                                            <span>High (50-80%)</span>
                                        </div>
                                        <div class=""legend-item"">
                                            <div class=""legend-color very-high""></div>
                                            <span>Very High (80-100%)</span>
                                        </div>
                                    </div>

                                    <div class=""footer"">
                                        <p>Heatmap automatically updates every 60 seconds. Click 'Refresh Now' to update immediately.</p>
                                        <p>ApacheNet Server | Powered by GeoHeatmap Generator</p>
                                    </div>
                                </div>

                                <script>
                                    // Auto-refresh heatmap every 60 seconds
                                    setInterval(refreshHeatmap, 60000);

                                    function refreshHeatmap() {{
                                        const img = document.getElementById('heatmapImage');
                                        img.src = '/heatmap?t=' + Date.now();
                                        console.log('Heatmap refreshed at ' + new Date().toLocaleTimeString());
                                    }}

                                    function downloadHeatmap() {{
                                        const link = document.createElement('a');
                                        link.href = '/heatmap';
                                        link.download = 'heatmap_' + new Date().toISOString().split('T')[0] + '.png';
                                        link.click();
                                    }}

                                    function viewStats() {{
                                        fetch('/heatmap.json')
                                            .then(r => r.json())
                                            .then(data => {{
                                                alert(
                                                    'Tracked Clients: ' + data.trackedClients + '\n' +
                                                    'Cached Maps: ' + data.cache.totalMaps + '\n' +
                                                    'Cache Size: ' + data.cache.totalSizeMB.toFixed(2) + ' MB'
                                                );
                                            }})
                                            .catch(e => alert('Error fetching stats: ' + e));
                                    }}

                                    console.log('Heatmap dashboard loaded');
                                </script>
                            </body>
                            </html>
                            ";

                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/html";
                        ctx.Response.Headers.Add("Cache-Control", "no-cache");
                        ctx.Response.Send(htmlContent).Wait();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[HeatmapRoute] Error serving HTML: {ex.Message}");
                        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        ctx.Response.ContentType = "text/html";
                        ctx.Response.Send("<h1>Error Loading Heatmap</h1><p>" + ex.Message + "</p>")
                            .Wait();
                        return true;
                    }
                },
            },
            // GET /heatmap/clear - Clear cached heatmap data (admin only)
            new()
            {
                Name = "Clear Heatmap Cache",
                UrlRegex = "^/heatmap/clear/?$",
                Method = "GET",
                Hosts = null,
                Callable = (ctx) =>
                {
                    string ipAddr = ctx.Request.Source.IpAddress;

                    // Only allow from localhost or configured admin IPs
                    if (
                        !string.IsNullOrEmpty(ipAddr)
                        && (
                            ApacheNetServerConfiguration.AllowedManagementIPs != null
                                && ApacheNetServerConfiguration.AllowedManagementIPs.Contains(
                                    ipAddr
                                )
                            || "::1".Equals(ipAddr)
                            || "127.0.0.1".Equals(ipAddr)
                            || "localhost".Equals(
                                ipAddr,
                                StringComparison.InvariantCultureIgnoreCase
                            )
                        )
                    )
                    {
                        try
                        {
                            ApacheNetHeatmapTracker.ClearTrackedData();

                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.Send(
                                    Newtonsoft.Json.JsonConvert.SerializeObject(
                                        new
                                        {
                                            status = "success",
                                            message = "Heatmap cache cleared",
                                        }
                                    )
                                )
                                .Wait();

                            LoggerAccessor.LogWarn(
                                $"[HeatmapRoute] Heatmap cache cleared by {ipAddr}"
                            );

                            return true;
                        }
                        catch (Exception ex)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.Send(
                                    Newtonsoft.Json.JsonConvert.SerializeObject(
                                        new { error = ex.Message }
                                    )
                                )
                                .Wait();
                            return true;
                        }
                    }

                    // Unauthorized
                    LoggerAccessor.LogError(
                        $"[HeatmapRoute] Unauthorized clear attempt from {ipAddr}"
                    );
                    ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    ctx.Response.Send().Wait();
                    return true;
                },
            },
        ];
    }
}
