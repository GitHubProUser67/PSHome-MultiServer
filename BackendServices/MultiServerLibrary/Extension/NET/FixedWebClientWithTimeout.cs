namespace System.Net
{
    public class FixedWebClientWithTimeout : FixedWebClient
    {
        public int Timeout { get; set; } = 5000; // milliseconds

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.Brotli;
            request.Timeout = Timeout;
            return request;
        }
    }
}
