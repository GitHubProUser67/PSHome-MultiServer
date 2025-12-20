using NetHasher;
using Newtonsoft.Json;
using System;
using System.Text;
using WebAPIService.GameServices.UBISOFT.Models.Hermes.v1;
using WebAPIService.GameServices.UBISOFT.Models.Hermes.v2;

namespace WebAPIService.GameServices.UBISOFT.HERMES_API.v1
{
    public static class V1SessionsClass
    {
        public const string FakeJWTToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"; // From CHATGPT if you ask.

        public static (string, string) HandleSessionPOST(byte[] PostData, string UbiAppId, string clientip, string regioncode)
        {
            if (PostData == null)
                return (null, null);

            V1SessionsRequest request = JsonConvert.DeserializeObject<V1SessionsRequest>(Encoding.UTF8.GetString(PostData));

            if (request != null && !string.IsNullOrEmpty(request.idOnPlatform))
            {
                string clientGUID = GenerateGuidFromId(request.idOnPlatform);

                switch (UbiAppId)
                {
                    case "594ee06a-24b7-4a9b-b409-b747cd45a141": // WatchDogs PS3
                        return (JsonConvert.SerializeObject(new V2SessionsResponse() // YEP, needs v2 resp for v1 req... :=)
                        {
                            clientIp = clientip,
                            clientIpCountry = regioncode,
                            nameOnPlatform = request.nameOnPlatform,
                            profileId = clientGUID,
                            sessionId = Guid.NewGuid().ToString(),
                            spaceId = "c3a2a2c0-8365-4942-862b-f9d27a97c00d",
                            ticket = FakeJWTToken,
                            userId = clientGUID,
                            username = request.nameOnPlatform,
                            platformType = "ps3"
                        }), "application/json; charset=utf-8");
                    case "9c4a1757-422b-458f-b4d2-5e623c911ba6": // WatchDogs PC
                        return (JsonConvert.SerializeObject(new V2SessionsResponse() // YEP, needs v2 resp for v1 req... :=)
                        {
                            clientIp = clientip,
                            clientIpCountry = regioncode,
                            nameOnPlatform = request.nameOnPlatform,
                            profileId = clientGUID,
                            sessionId = Guid.NewGuid().ToString(),
                            spaceId = "c8237ba1-f3a7-4a93-acb6-a23044c4f0cf",
                            ticket = FakeJWTToken,
                            userId = clientGUID,
                            username = request.nameOnPlatform,
                            platformType = "PC"
                        }), "application/json; charset=utf-8");
                }
            }

            return (null, null);
        }

        private static string GenerateGuidFromId(string id)
        {
            // Create a GUID from the hash
            byte[] guidBytes = new byte[16];
            Array.Copy(DotNetHasher.ComputeMD5(Encoding.UTF8.GetBytes(id + "Ub1S0ft!!")), guidBytes, 16); // Take the first 16 bytes of the hash
            return new Guid(guidBytes).ToString();
        }
    }
}
