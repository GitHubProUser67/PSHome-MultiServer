using CustomLogger;
using MultiServerLibrary.Extension;
using System;
using System.Linq;
using System.Net;
#if NET7_0_OR_GREATER
using System.Net.Http;
#endif
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiServerLibrary.AdBlocker
{
    public class AdGuardFilterChecker
    {
        public bool isLoaded = false;

        private string[] excludedUrls;
        private Regex[] regexRules;

        // Download the AdGuard DNS filter list and parse the rules
        public async Task DownloadAndParseFilterListAsync()
        {
            if (isLoaded)
                return;

            const string adGuardFilterUrl = "https://adguardteam.github.io/AdGuardSDNSFilter/Filters/filter.txt";

            excludedUrls = Array.Empty<string>();
            regexRules = Array.Empty<Regex>();

            try
            {
#if NET7_0_OR_GREATER
            using (HttpClient client = new HttpClient())
            {
                foreach (string line in (await client.GetStringAsync(adGuardFilterUrl).ConfigureAwait(false)).Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
#else
#pragma warning disable
                using (GZipWebClient client = new GZipWebClient())
#pragma warning restore
                {
                    foreach (string line in (await client.DownloadStringTaskAsync(adGuardFilterUrl).ConfigureAwait(false)).Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
#endif
                    {
                        // If the line starts with "||" or "://", it is an excluded URL
                        if (line.StartsWith("||"))
                        {
                            string processedLine = line.Substring(2).Trim();
                            int lastSpaceIndex = processedLine.LastIndexOf('^');

                            if (lastSpaceIndex != -1)
                                excludedUrls.AddElementToArray(processedLine.Substring(0, lastSpaceIndex));
                            else
                                excludedUrls.AddElementToArray(processedLine);
                        }
                        else if (line.StartsWith("://"))
                        {
                            string processedLine = line.Substring(3).Trim();
                            int lastSpaceIndex = processedLine.LastIndexOf('^');

                            if (lastSpaceIndex != -1)
                                excludedUrls.AddElementToArray(processedLine.Substring(0, lastSpaceIndex));
                            else
                                excludedUrls.AddElementToArray(processedLine);
                        }
                        // If the line starts with "/^", it is a regex rule
                        else if (line.StartsWith("/^"))
                        {
                            try
                            {
                                // Validate and compile the regex (catching any potential errors)
                                regexRules.AddElementToArray(new Regex(FormatRegexStr(line.Trim())));
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[AdGuardFilterChecker] - Invalid regex pattern: {ex.Message}");
                            }
                        }
                    }
                }

                isLoaded = true;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[AdGuardFilterChecker] - Error while downloading the Adguard rules: {ex.Message}");
            }
        }

        public bool IsDomainRefused(string url)
        {
            return regexRules.Any(regex => regex.IsMatch(url))
                ^ excludedUrls.Any(excludedUrl => url.Contains(excludedUrl));
        }

        private string FormatRegexStr(string regex)
        {
            if (regex.Length <= 2)
                return regex;

            return $@"{regex.Substring(1, regex.Length - 2)}";
        }
    }
}
