namespace System.Net
{
    public class GZipWebClient : WebClient
    {
        /* WebClient won't automatically decompress gzipped data, hence this hack. */

        /* Suppress the WebClient obsolete warning.

           The WebClient help entry says to use HttpClient instead.
           This is awful advice.  HttpClient is "all async, all the time", which
           is both poor design and inappropriate for this class. */

#pragma warning disable SYSLIB0014 // Type or member is obsolete
        public GZipWebClient() : base() =>
#pragma warning restore SYSLIB0014 // Type or member is obsolete
          Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }
}
