using System.Net;
using WebAPIService.WebServices.WebCrypto;

namespace Horizon.MUM
{
    public class MumClient
    {
        public static string? GetServerResult(string ip, ushort port, string command, string key)
        {
            try
            {
#pragma warning disable // NET 6.0 and lower has a bug where GetAsync() is EXTREMLY slow to operate (https://github.com/dotnet/runtime/issues/65375).
                FixedWebClient client = new();
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
                client.Headers.Add("method", "GET");
                client.Headers.Add("content-type", "text/xml; charset=UTF-8");
                return WebCryptoClass.DecryptCTR(client.DownloadStringTaskAsync($"http://{ip}:{port}/{command}/").Result, key, MumUtils.ConfigIV);
#pragma warning restore
            }
            catch (Exception)
            {
                // Not Important.
            }

            return null;
        }
    }
}
