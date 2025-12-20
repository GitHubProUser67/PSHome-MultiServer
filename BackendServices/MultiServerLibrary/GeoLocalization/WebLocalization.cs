using System.Collections.Generic;
using System.Globalization;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MultiServerLibrary.GeoLocalization
{
    public static class WebLocalization
    {
        private static async Task<IPInfo> GetIPInfoFromIP(string ip)
        {
#pragma warning disable
            using (FixedWebClient client = new FixedWebClient())
#pragma warning restore
            {
                try
                {
                    return JsonSerializer.Deserialize<IPInfo>(await client.DownloadStringTaskAsync($"https://ipinfo.io/{ip}/json").ConfigureAwait(false));
                }
                catch (Exception e)
                {
                    CustomLogger.LoggerAccessor.LogError($"[WebLocalization] - GetIPInfoFromIP: Error fetching IPInfo data from the web API. (Exception: {e})");
                }
            }

            return null;
        }

        private static async Task<Dictionary<GeoData, double[]>> GetNominatimDetails(string country, string city = "")
        {
            Dictionary<GeoData, double[]> geoDataDic = new Dictionary<GeoData, double[]>();

#pragma warning disable
            using (FixedWebClient client = new FixedWebClient())
#pragma warning restore
            {
                client.UseDefaultCredentials = false;

                // Add these fake Chrome headers to the GZipWebClient request to avoid our client being flagged as a bot.
                client.Headers.Add("Upgrade-Insecure-Requests", "1");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36");
                client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                client.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"134\", \"Not:A-Brand\";v=\"24\", \"Google Chrome\";v=\"134\"");
                client.Headers.Add("sec-ch-ua-mobile", "?0");
                client.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                client.Headers.Add("Accept-Language", "fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7,es;q=0.6");

                try
                {
                    foreach (GeoData geoData in JsonSerializer.Deserialize<List<GeoData>>(await client.DownloadStringTaskAsync($"https://nominatim.openstreetmap.org/search?country={country}{(string.IsNullOrEmpty(city) ? string.Empty : "&city=" + country)}&format=json&addressdetails=1").ConfigureAwait(false)))
                    {
                        // Extract the latitude and longitude to a more friendly format.
                        geoDataDic.Add(geoData, new double[] { Convert.ToDouble(geoData.Lat, CultureInfo.InvariantCulture), Convert.ToDouble(geoData.Lon, CultureInfo.InvariantCulture) });
                    }
                }
                catch (Exception e)
                {
                    CustomLogger.LoggerAccessor.LogError($"[WebLocalization] - GetNominatimDetails: Error fetching coordinates from the web API. (Exception: {e})");
                }
            }

            return geoDataDic;
        }

        public static async Task<string> GetOpenStreetMapUrl(string clientip, int zoomLevel = 10)
        {
            IPInfo ipInfo = await GetIPInfoFromIP(clientip).ConfigureAwait(false);
            if (ipInfo != null && !string.IsNullOrEmpty(ipInfo.Loc))
            {
                string[] locLat = ipInfo.Loc.Split(',');
                if (locLat.Length == 2)
                    return $"https://www.openstreetmap.org/#map={zoomLevel}/{locLat[0]}/{locLat[1]}";
                else
                    CustomLogger.LoggerAccessor.LogError($"[WebLocalization] - GetOpenStreetMapUrl: invalid json data fetched: {ipInfo}");
            }
            return null;
        }
    }
}
