using NetCoreServer;
using NetHasher;
using MultiServerLibrary.HTTP;
using System.Text;
using System.Text.Json;

namespace SSFWServer.Services
{
    public class ClanService
    {
        private readonly string? _sessionid;

        public ClanService(string sessionid)
        {
            _sessionid = sessionid;
        }

        // Handles GET,POST,DELETE on this function
        public HttpResponse HandleClanDetailsService(HttpRequest req, HttpResponse res, string absolutepath)
        {
            string filePath = $"{SSFWServerConfiguration.SSFWStaticFolder}{HTTPProcessor.ParseUriFromAbsolutePath(absolutepath).AbsolutePath}.json";

            if (req.Method == HttpMethod.Post.ToString())
            {
                try
                {
                    if (JsonDocument.Parse(req.Body).RootElement.TryGetProperty("sceneObjectId", out JsonElement idElement))
                    {
                        string? psnClanId = absolutepath.Split("/").LastOrDefault();
                        string? directoryPath = Path.GetDirectoryName(filePath);

                        if (string.IsNullOrEmpty(psnClanId) || string.IsNullOrEmpty(directoryPath))
                            throw new Exception();

                        Directory.CreateDirectory(directoryPath);

                        // TODO, extract the proper region.
                        string jsonToWrite = $@"{{
""region"": ""en-US"",
""message"": ""OK"",
""result"": 0,
""psnClanId"": {psnClanId},
""sceneObjectId"": ""{idElement.GetString()!}"",
""personId"": ""{_sessionid}"",
""clanId"": ""{DotNetHasher.ComputeMD5String(Encoding.UTF8.GetBytes(psnClanId))}""
}}";
                        File.WriteAllText(filePath, jsonToWrite);

                        return res.MakeGetResponse(jsonToWrite, "application/json");
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
                // If clanId exist, we check json and return that back, otherwise not found so Home POST default
                if (File.Exists(filePath))
                    return res.MakeGetResponse(File.ReadAllText(filePath), "application/json");

                return res.MakeErrorResponse(404, "Not Found");
            }

            // Delete clan details if clan requested DELETE method
            try
            {
                if (File.Exists(filePath))
                {
                    string? sceneObjectId = JsonDocument.Parse(File.ReadAllText(filePath)).RootElement.GetProperty("sceneObjectId").GetString() ?? string.Empty;
                    File.Delete(filePath);
                    return res.MakeGetResponse($"{{\"sceneObjectIds\": [\"{sceneObjectId}\"] }}", "application/json");
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