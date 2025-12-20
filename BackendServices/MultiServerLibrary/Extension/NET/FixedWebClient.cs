using FixedSsl;
using MultiServerLibrary;

namespace System.Net
{
    public class FixedWebClient : WebClient
    {
        private static readonly Type _dummy = typeof(SslSocket); // Dummy type for service point initialization.

        private const bool bypassProxyLocalHost = false; // still use the proxy for local addresses

        /* WebClient won't automatically decompress compressed data and has no proxy abstraction layer, hence this hack.

           Suppress the WebClient obsolete warning.

           The WebClient help entry says to use HttpClient instead.
           This is awful advice.  HttpClient is "all async, all the time", which
           is both poor design and inappropriate for this class. */

#pragma warning disable SYSLIB0014 // Type or member is obsolete
        public FixedWebClient() : base()
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        {
            string proxyHost = MultiServerLibraryConfiguration.ProxyHost;
            ushort proxyPort = MultiServerLibraryConfiguration.ProxyPort;
            Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            if (!string.IsNullOrEmpty(proxyHost) && proxyPort != 0)
            {
                Proxy = new WebProxy(proxyHost, proxyPort) { UseDefaultCredentials = false, BypassProxyOnLocal = bypassProxyLocalHost };
                Credentials = new NetworkCredential(MultiServerLibraryConfiguration.ProxyUserName, MultiServerLibraryConfiguration.ProxyPassword);
            }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.Brotli;
            return request;
        }
    }
}
