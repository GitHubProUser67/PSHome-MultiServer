using System.Net;
using CastleLibrary.FixedSsl;

namespace MultiServerLibrary.Extension.NET
{
    public class FixedWebClient : WebClient
    {
        private static readonly Type _dummy = typeof(SslSocket); // Dummy type for service point initialization.

        private const bool bypassProxyLocalHost = false; // still use the proxy for local addresses

        private bool _pipelineSupport = false;
        private bool _keepalive = false;

        private string _method = null;

        public bool PipelineSupport
        {
            get { return _pipelineSupport; }
            set { _pipelineSupport = value; }
        }

        public bool KeepAlive
        {
            get { return _keepalive; }
            set { _keepalive = value; }
        }

        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public HttpStatusCode? StatusCode { get; private set; }

        /* WebClient won't automatically do a bunch of stuff, hence this class.
         *
           The WebClient help entry says to use HttpClient instead.
           This is awful advice.  HttpClient is "all async, all the time", which
           is both poor design and inappropriate for this class. */

#pragma warning disable SYSLIB0014 // Type or member is obsolete
        public FixedWebClient()
            : base()
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        {
            string proxyHost = MultiServerLibraryConfiguration.ProxyHost;
            ushort proxyPort = MultiServerLibraryConfiguration.ProxyPort;
            Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            if (!string.IsNullOrEmpty(proxyHost) && proxyPort != 0)
            {
                Proxy = new WebProxy(proxyHost, proxyPort)
                {
                    UseDefaultCredentials = false,
                    BypassProxyOnLocal = bypassProxyLocalHost,
                };
                Credentials = new NetworkCredential(
                    MultiServerLibraryConfiguration.ProxyUserName,
                    MultiServerLibraryConfiguration.ProxyPassword
                );
            }
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            if (_pipelineSupport)
            {
                // Required for pipelining
                _keepalive = true;
                request.ProtocolVersion = HttpVersion.Version11;

                request.Pipelined = true;
            }
            request.KeepAlive = _keepalive;
            if (!string.IsNullOrEmpty(_method))
                request.Method = _method;
            request.AutomaticDecompression =
                DecompressionMethods.Deflate
                | DecompressionMethods.GZip
                | DecompressionMethods.Brotli;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);

            if (response is HttpWebResponse httpResponse)
                StatusCode = httpResponse.StatusCode;

            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            var response = base.GetWebResponse(request, result);

            if (response is HttpWebResponse httpResponse)
                StatusCode = httpResponse.StatusCode;

            return response;
        }
    }
}
