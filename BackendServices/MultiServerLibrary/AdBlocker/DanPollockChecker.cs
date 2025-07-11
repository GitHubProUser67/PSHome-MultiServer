using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
#if NET7_0_OR_GREATER
using System.Net.Http;
#endif
using System.Threading.Tasks;

namespace MultiServerLibrary.AdBlocker
{
    public class DanPollockChecker
    {
        public bool isLoaded = false;

        private ConcurrentDictionary<string, IPAddress> UrlsDic;

        // Download the DanPollock hosts file and parse the rules
        public async Task DownloadAndParseFilterListAsync(bool asLocalHost = false)
        {
            if (isLoaded)
                return;

            string danpollockFilterUrl = asLocalHost ? "https://someonewhocares.org/hosts/" : "https://someonewhocares.org/hosts/zero/";

            UrlsDic = new ConcurrentDictionary<string, IPAddress>();

            try
            {
#if NET7_0_OR_GREATER
            using (HttpClient client = new HttpClient())
            {
                string content = await client.GetStringAsync(danpollockFilterUrl).ConfigureAwait(false);
#else
#pragma warning disable
                using (GZipWebClient client = new GZipWebClient())
#pragma warning restore
                {
                    string content = await client.DownloadStringTaskAsync(danpollockFilterUrl).ConfigureAwait(false);
#endif
                    Parallel.ForEach(content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries), line =>
                    {
                        // Exclude invalid lines on the webpage.
                        if (!line.StartsWith("#") && !line.StartsWith("<") && !line.StartsWith("&"))
                        {
                            string[] splitedLine = line.Trim().Replace("\t", string.Empty).Split(' ');
                            if (splitedLine.Length >= 2 && IPAddress.TryParse(splitedLine[0], out IPAddress targetIp) && targetIp != null)
                                UrlsDic.TryAdd(splitedLine[1], targetIp);
                        }
                    });
                }

                isLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DanPollockChecker] - Error while downloading the DanPollock hosts file: {ex.Message}");
            }
        }

        public IPAddress GetDomainIP(string domain)
        {
            return UrlsDic.FirstOrDefault(kv => kv.Key.Equals(domain)).Value;
        }
    }
}
