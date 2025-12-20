using System;
using System.Threading.Tasks;
using System.Net;

namespace MultiServerLibrary.HTTP
{
    public static class DefaultHTMLPages
    {
        public static Task<string> GenerateErrorPageAsync(HttpStatusCode status, string absolutepathUrl,
            string urlBase, string HttpRootFolder, string serverSignature, string host,
            int serverPort, Exception ex = null)
        {
            string HTMLContent = $@"<!DOCTYPE html PUBLIC ""-//IETF//DTD HTML 2.0//EN"">
                                <html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=windows-1252"">
                                <title>{(int)status} {status}</title>
                                <style>.hiclass {{background-color: rgb(51, 144, 255); color: white}}</style>
                                <script>
                                    function toggleList() {{
                                        var list = document.getElementById('urlList');
                                        list.style.display = list.style.display === 'none' ? 'block' : 'none';
                                    }}
                                </script>
                                </head><body>
                                <h1>{status}</h1>";

            if (status == HttpStatusCode.NotFound)
            {
                HTMLContent += $@"<p>The requested URL was not found on this server.</p>
                                  <hr>
                                  <address>{serverSignature} Server at {host} Port {serverPort}</address></body></html>";

                return Task.FromResult(HTMLContent);
            }
            else if (status == HttpStatusCode.InternalServerError)
            {
                HTMLContent += $@"<p>An unexpected error occurred on the server.</p>";

                if (ex != null)
                    HTMLContent += $@"<p><strong>Error Details:</strong></p>
                              <pre>{ex.Message}</pre>
                              <p><strong>Help Link:</strong></p>
                              <pre>{ex.HelpLink}</pre>
                              <p><strong>Stack Trace:</strong></p>
                              <pre>{ex.StackTrace}</pre>";
            }

            return Task.FromResult(HTMLContent += $@"<hr>
                          <address>{serverSignature} Server at {host} Port {serverPort}</address></body></html>");
        }
    }
}
