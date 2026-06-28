using System.Collections.Concurrent;
using System.Net;
using MultiServerLibrary.Extension.NET;

namespace MultiServerLibrary.AdBlocker
{
    public class DanPollockChecker
    {
        public bool isLoaded = false;

        private ConcurrentDictionary<string, IPAddress> UrlsDic;
        private static readonly char[] separator = new[] { '\n', '\r' };

        // Download the DanPollock hosts file and parse the rules
        public async Task DownloadAndParseFilterListAsync(bool asLocalHost = false)
        {
            if (isLoaded)
                return;

            var danpollockFilterUrl = asLocalHost
                ? "https://someonewhocares.org/hosts/"
                : "https://someonewhocares.org/hosts/zero/";

            UrlsDic = new ConcurrentDictionary<string, IPAddress>();

            try
            {
#pragma warning disable
                using (FixedWebClient client = new FixedWebClient())
#pragma warning restore
                {
                    var content = await client
                        .DownloadStringTaskAsync(danpollockFilterUrl)
                        .ConfigureAwait(false);
                    Parallel.ForEach(
                        content.Split(separator, StringSplitOptions.RemoveEmptyEntries),
                        line =>
                        {
                            // Exclude invalid lines on the webpage.
                            if (
                                !line.StartsWith("#")
                                && !line.StartsWith("<")
                                && !line.StartsWith("&")
                            )
                            {
                                var splitedLine = line.Trim()
                                    .Replace("\t", string.Empty)
                                    .Split(' ');
                                if (
                                    splitedLine.Length >= 2
                                    && IPAddress.TryParse(splitedLine[0], out var targetIp)
                                    && targetIp != null
                                )
                                    UrlsDic.TryAdd(splitedLine[1], targetIp);
                            }
                        }
                    );
                }

                isLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[DanPollockChecker] - Error while downloading the DanPollock hosts file: {ex.Message}"
                );
            }
        }

        public IPAddress GetDomainIP(string domain)
        {
            return UrlsDic.FirstOrDefault(kv => kv.Key.Equals(domain)).Value;
        }
    }
}
