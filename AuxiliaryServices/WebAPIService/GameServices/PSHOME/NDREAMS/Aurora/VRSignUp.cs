using System.Text;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Aurora
{
    public static class VRSignUp
    {
        public static string ProcessVRSignUp(byte[] PostData, string ContentType, string apipath)
        {
            var email = string.Empty;
            var username = string.Empty;
            var hash = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    email = data.GetParameterValue("email");
                    username = data.GetParameterValue("username");
                    hash = data.GetParameterValue("hash");

                    ms.Flush();
                }

                var ExpectedHash = CastleLibrary
                    .NetHasher.DotNetHasher.ComputeSHA1String(
                        Encoding.UTF8.GetBytes(
                            email + "_" + username + "_" + "V305iSReuFCeRvLpt2mMh83nkeV0p9pl"
                        )
                    )
                    .ToLower();

                if (hash.Equals(ExpectedHash))
                {
                    Directory.CreateDirectory(apipath + "/NDREAMS/Aurora/VRSignUp");

                    var SignedUpProfilePath = apipath + $"/NDREAMS/Aurora/VRSignUp/{username}.txt";

                    if (File.Exists(SignedUpProfilePath))
                    {
                        var Extractedemail = File.ReadAllText(SignedUpProfilePath)
                            .Replace("email=", string.Empty);

                        if (string.IsNullOrEmpty(Extractedemail))
                        {
                            CustomLogger.LoggerAccessor.LogWarn(
                                $"[nDreams] - VRSignUp: Profile:{SignedUpProfilePath} has an invalid format! Overwritting..."
                            );
                            File.WriteAllText(SignedUpProfilePath, $"email={email}");
                            return $"{{\"success\":\"true\",\"reward\":\"true\"}}";
                        }
                        else
                        {
                            if (Extractedemail == email)
                                return $"{{\"success\":\"true\",\"reward\":\"false\"}}";
                            else
                            {
                                File.WriteAllText(SignedUpProfilePath, $"email={email}");
                                return $"{{\"success\":\"true\",\"reward\":\"true\"}}";
                            }
                        }
                    }
                    else
                    {
                        File.WriteAllText(SignedUpProfilePath, $"email={email}");
                        return $"{{\"success\":\"true\",\"reward\":\"true\"}}";
                    }
                }
                else
                {
                    var errMsg =
                        $"[nDreams] - VRSignUp: invalid hash sent! Received:{hash} Expected:{ExpectedHash}";
                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                    return $"{{\"success\":\"false\",\"error\":\"{errMsg}\"}}";
                }
            }

            return null;
        }
    }
}
