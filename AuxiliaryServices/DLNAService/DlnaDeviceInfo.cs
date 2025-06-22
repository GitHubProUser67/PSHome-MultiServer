using System;
#if NET7_0_OR_GREATER 
using System.Net.Http;
#else
using System.Net;
#endif
using System.Text.RegularExpressions;

namespace DLNAService
{
    public class FetchDLNARemote
    {
        public static string FetchXmlContent(string url)
        {
            try
            {
#if NET7_0_OR_GREATER
                return new HttpClient().GetStringAsync(url).Result.Replace("status=ok", string.Empty);
#else
#pragma warning disable // NET 6.0 and lower has a bug where GetAsync() is EXTREMLY slow to operate (https://github.com/dotnet/runtime/issues/65375).
                return new WebClient().DownloadStringTaskAsync(url).Result.Replace("status=ok", string.Empty);
#pragma warning restore
#endif
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[FetchDNLARemote] - An exception was thrown while fetching XML from {url} : {ex}");
                return null;
            }
        }

        public static DlnaDeviceInfo ParseXml(string xmlContent, string url)
        {
            // Parse the URL
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                return new DlnaDeviceInfo
                {
                    Path = url,
                    FriendlyName = GetFriendlyName(xmlContent) ?? "Not Set",
                    Logo = GetImagesUrl(xmlContent, uri.GetLeftPart(UriPartial.Authority))
                };
            }

            return new DlnaDeviceInfo
            {
                Path = url,
                FriendlyName = GetFriendlyName(xmlContent) ?? "Not Set",
                Logo = GetImagesUrl(xmlContent, url)
            };
        }

        private static string GetFriendlyName(string xmlContent)
        {
            // Use Regex.Match to find the first match
            Match match = Regex.Match(xmlContent, @"<friendlyName>(.*?)</friendlyName>");

            // If a match is found, return the captured value
            if (match.Success)
                return match.Groups[1].Value;

            // Return an empty string if no match is found
            return null;
        }

        private static string GetImagesUrl(string xmlContent, string RawUrl)
        {
            // Match the pattern in the XML string
            MatchCollection matches = Regex.Matches(xmlContent, @"<url>(.*?)<\/url>");

            // Extract URLs and concatenate with a semicolon delimiter
            string[] urlArray = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                urlArray[i] = RawUrl + matches[i].Groups[1].Value;
            }

            // Check if any URLs are found
            if (matches.Count > 0)
                // Concatenate URLs with a semicolon delimiter
                return string.Join(";", urlArray);

            return "Not Set";
        }
    }

    public class DlnaDeviceInfo
    {
        public string Path { get; set; }
        public string FriendlyName { get; set; }
        public string Logo { get; set; }
    }
}