using System.Collections.Concurrent;
using System.Globalization;

namespace MultiServerLibrary.GeoLocalization.Heatmap
{
    /// <summary>
    /// Advanced heatmap generator with support for time-based tracking and live updates.
    /// </summary>
    public class ClientUsageHeatmapTracker(
        int mapWidth = 1600,
        int mapHeight = 800
    )
    {
        private readonly GeoHeatmapGenerator _generator = new(
            mapWidth,
            mapHeight
        );
        private readonly ConcurrentDictionary<
            string,
            (double latitude, double longitude)
        > _cachedGeoData = new();

        /// <summary>
        /// Adds or updates an IP with its geolocation data.
        /// </summary>
        public void TrackIP(IPInfo ipInfo)
        {
            if (ipInfo == null)
                return;

            if (!string.IsNullOrEmpty(ipInfo.Loc))
            {
                var parts = ipInfo.Loc.Split(',');
                if (parts.Length == 2)
                {
                    try
                    {
                        _cachedGeoData[ipInfo.Ip] = (
                            double.Parse(parts[0], CultureInfo.InvariantCulture),
                            double.Parse(parts[1], CultureInfo.InvariantCulture)
                        );
                    }
                    catch
                    {
                        // Not Important.
                    }
                }
            }
        }

        /// <summary>
        /// Generates a heatmap from all currently tracked IPs.
        /// </summary>
        public async Task<byte[]> GenerateCurrentHeatmapAsync()
        {
            if (_cachedGeoData.IsEmpty)
                throw new InvalidOperationException(
                    "[ClientUsageHeatmapTracker] - No tracked IP data available for heatmap generation"
                );

            return await _generator
                .GenerateHeatmapAsync(_cachedGeoData.ToDictionary(
                        kvp => kvp.Key,
                        kvp => (kvp.Value.latitude, kvp.Value.longitude)
                    ))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Clears all cached geolocation data.
        /// </summary>
        public void ClearCache()
        {
            _cachedGeoData.Clear();
        }

        /// <summary>
        /// Gets the number of tracked IPs.
        /// </summary>
        public int TrackedIPCount => _cachedGeoData.Count;
    }
}
