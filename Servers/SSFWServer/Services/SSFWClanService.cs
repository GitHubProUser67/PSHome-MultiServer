using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NetCoreServer;
using NetHasher;

namespace SSFWServer.Services
{
    public class SSFWClanService
    {
        private readonly string? _sessionid;

        public SSFWClanService(string sessionid)
        {
            _sessionid = sessionid;
        }

        // Handles both GET/POST on this
        public HttpResponse HandleClanDetailsService(HttpRequest req, HttpResponse res, string absolutepath)
        {
            string filePath = $"{SSFWServerConfiguration.SSFWStaticFolder}{new Uri(absolutepath).AbsolutePath}" + ".json";

            if (req.Method == HttpMethod.Post.ToString())
            {
                try
                {
                    using JsonDocument doc = JsonDocument.Parse(req.Body);

                    if (doc.RootElement.TryGetProperty("sceneObjectId", out JsonElement idElement))
                    {
                        Directory.CreateDirectory(absolutepath);

                        string jsonToWrite = $@"{{
                            ""region"":""en-US"",
                            ""message"":""OK"",
                            ""result"":0,
                            ""psnClanId"":{absolutepath.Split("/").LastOrDefault()},
                            ""sceneObjectId"":""{idElement.GetString()!}"",
                            ""personId"":""{_sessionid}"",
                            ""clanId"":""{DotNetHasher.ComputeMD5String(Encoding.UTF8.GetBytes(absolutepath.Split("/").LastOrDefault()!))}""
                            }}";

                        File.WriteAllText(filePath, jsonToWrite);

                        return res.MakeGetResponse($@"{jsonToWrite}", "application/json");
                    }

                }
                catch
                {
                    // Not Important.
                }

                return res.MakeErrorResponse(400);
            }
            else if (req.Method == HttpMethod.Get.ToString()) // GET ONLY
            {
                // If clanid exist, we check json and return that back, otherwise not found so Home POST default
                if (File.Exists(filePath))
                    return res.MakeGetResponse($@"{File.ReadAllText(filePath)}", "application/json");

                return res.MakeErrorResponse(404, "Not Found");
            }

            // Delete clan details
            try
            {
                if (File.Exists(filePath))
                {
                    using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(filePath));

                    string? sceneObjectId = doc.RootElement.GetProperty("sceneObjectId").GetString() ?? string.Empty;

                    File.Delete(filePath);

                    return res.MakeGetResponse($"{{\"sceneObjectIds\":[\"{sceneObjectId}\"]}}", "application/json");
                }
            }
            catch
            {
                // Not Important.
            }

            return res.MakeErrorResponse();
        }
    }
}
