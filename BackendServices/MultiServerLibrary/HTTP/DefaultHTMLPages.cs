using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace MultiServerLibrary.HTTP
{
    public static class DefaultHTMLPages
    {
        private static readonly ConcurrentDictionary<string, Tuple<DateTime, List<string>>> AINotFoundGuessingResultCache = new ConcurrentDictionary<string, Tuple<DateTime, List<string>>>();
        private static readonly SemaphoreSlim searchSemaphore = new SemaphoreSlim(Environment.ProcessorCount); // Limit to number-of-CPU-cores concurrent searches

        public static Task<string> GenerateErrorPageAsync(HttpStatusCode status, string absolutepathUrl,
            string urlBase, string HttpRootFolder, string serverSignature, string host,
            int serverPort, bool AIAssistant, Exception ex = null)
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
                if (AIAssistant && UrlAnalyzer.IsAvailable())
                {
                    List<string> Urls = null;

                    // The idea behind this logic, is to prevent too much processing for the same url.
                    if (AINotFoundGuessingResultCache.TryGetValue(absolutepathUrl, out Tuple<DateTime, List<string>> value) && (DateTime.Now - value.Item1).TotalMinutes <= 30) // We renew cache entry after 30 minutes of validity.
                        Urls = value.Item2;
                    else if (searchSemaphore.Wait(0)) // We not await for the semaphore, rather we check if yes or not we can enter.
                    {
                        Urls = UrlAnalyzer.GenerateUrlsSuggestions(absolutepathUrl, HttpRootFolder, 0.1)?.Where(url => url != null).ToList();

                        if (Urls != null)
                        {
                            if (AINotFoundGuessingResultCache.ContainsKey(absolutepathUrl))
                                AINotFoundGuessingResultCache[absolutepathUrl] = Tuple.Create(DateTime.Now, Urls);
                            else
                                AINotFoundGuessingResultCache.TryAdd(absolutepathUrl, Tuple.Create(DateTime.Now, Urls));
                        }

                        searchSemaphore.Release();
                    }

                    if (Urls != null && Urls.Count != 0)
                    {
                        HTMLContent += $@"<p>The requested URL was not found on this server, but we found some ressources you might want.</p>
                          <button onclick=""toggleList()"">Toggle Suggested URL List</button>
                          <ul id=""urlList"" style=""display: none;"">";

                        foreach (string Url in Urls)
                        {
                            HTMLContent += $"<li><a href=\"{urlBase + Url}\">{urlBase + Url}</a></li>";
                        }

                        HTMLContent += $@"</ul><hr><address>{serverSignature} Server at {host} Port {serverPort}</address></body></html>";
                    }
                    else
                        HTMLContent += $@"<p>The requested URL was not found on this server, but there might be some suggestions in the future, please check again later.</p>
                          <hr>
                          <address>{serverSignature} Server at {host} Port {serverPort}</address></body></html>";
                }
                else
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
