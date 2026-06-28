using System.Net;

namespace MultiServerLibrary.Extension.NET
{
    public class FixedWebClientWithTimeout : FixedWebClient
    {
        private bool _pipelineSupport = false;
        private bool _keepalive = false;

        private string _method = null;

        public int Timeout { get; set; } = 5000; // milliseconds

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
            request.Timeout = Timeout;
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
