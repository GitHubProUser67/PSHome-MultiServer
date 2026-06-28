using System.Net;
using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.GeoLocalization.Heatmap;

namespace ApacheNet
{
    /// <summary>
    /// Global heatmap tracker for ApacheNet server.
    /// Tracks all incoming client requests and generates world heatmaps.
    /// </summary>
    public static class ApacheNetHeatmapTracker
    {
        public static bool IsEnabled { get; private set; } = false;

        private static ClientUsageHeatmapTracker? _tracker;
        private static readonly Lock _lockObject = new Lock();

        /// <summary>
        /// Initializes the heatmap tracker system.
        /// Should be called once during server startup.
        /// </summary>
        public static void Initialize()
        {
            lock (_lockObject)
            {
                _tracker = new ClientUsageHeatmapTracker(1600, 800); // Static for now.

                LoggerAccessor.LogInfo("[ApacheNetHeatmapTracker] - Initialized successfully");

                IsEnabled = true;
            }
        }

        /// <summary>
        /// Tracks an incoming client request.
        /// Extracts geolocation data from the client IP address.
        /// </summary>
        public static void TrackClientRequest(IPAddress clientIP)
        {
            // Skip localhost and private IPs for heatmap
            if (InternetProtocolUtils.IsPrivate(clientIP) || _tracker == null)
                return;

            try
            {
                var ipInfo = new IPInfo { Ip = clientIP.ToString() };

                // Try to get location from GeoIP database
                try
                {
                    var geoIP = GeoIP.Instance;
                    if (geoIP != null && geoIP.CityReader != null)
                    {
                        var response = geoIP.CityReader.City(clientIP);
                        if (response?.Location != null)
                        {
                            ipInfo.Loc =
                                $"{response.Location.Latitude.ToString().Replace(",", ".")},{response.Location.Longitude.ToString().Replace(",", ".")}";
                            ipInfo.City = response.City?.Name;
                            ipInfo.Country = response.Country?.IsoCode;
                        }
                    }
                }
                catch
                {
                    // If geolocation lookup fails, continue without it
                }

                _tracker.TrackIP(ipInfo);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[ApacheNetHeatmapTracker] - Error tracking IP {clientIP}: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Gets the current heatmap as bytes.
        /// </summary>
        public static async Task<byte[]> GetCurrentHeatmapAsync()
        {
            if (_tracker == null)
                throw new InvalidOperationException(
                    "[ApacheNetHeatmapTracker] - Heatmap tracker not initialized"
                );

            return await _tracker.GenerateCurrentHeatmapAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets heatmap statistics.
        /// </summary>
        public static int GetTrackedIPCount()
        {
            return _tracker?.TrackedIPCount ?? 0;
        }

        /// <summary>
        /// Clears all tracked data.
        /// </summary>
        public static void ClearTrackedData()
        {
            _tracker?.ClearCache();
            LoggerAccessor.LogDebug("[ApacheNetHeatmapTracker] - Cleared all tracked data");
        }
    }
}
