using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Mansion13
{
    public class Mansion13Class
    {
        private static readonly Lock _Lock = new Lock();

        public static string ProcessFragments(byte[] PostData, string ContentType, string apiPath)
        {
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                bool isAvailable = true;
                string func = string.Empty,
                    name = string.Empty,
                    existingfileContents = string.Empty;
                int fragCount = 0;

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    name = data.GetParameterValue("name");
                }

                string playerProfilePath = apiPath + $"/NDREAMS/Mansion13/{name}.txt";
                string isAvailableFile = apiPath + $"/NDREAMS/Mansion13/available.txt";

                lock (_Lock)
                {
                    if (!File.Exists(isAvailableFile))
                    {
                        File.WriteAllText(isAvailableFile, "true");
#if DEBUG
                        LoggerAccessor.LogInfo($"[Mansion13Class] - IsAvailable? {isAvailable}");
#endif
                    }
                    else
                        isAvailable = Convert.ToBoolean(File.ReadAllText(isAvailableFile));
                }

                switch (func)
                {
                    case "gotfragment":
                    {
                        if (File.Exists(playerProfilePath))
                        {
#if DEBUG
                            LoggerAccessor.LogInfo(
                                $"[Mansion13Class] - Existing {name}'s new fragment file detected, incrementing."
                            );
#endif
                            existingfileContents = File.ReadAllText(playerProfilePath);
                            fragCount = Convert.ToInt32(existingfileContents) + 1;

                            File.WriteAllText(playerProfilePath, fragCount.ToString());
                        }
                        else
                        {
                            File.WriteAllText(playerProfilePath, fragCount.ToString());
#if DEBUG
                            LoggerAccessor.LogInfo(
                                $"[Mansion13Class] - Writing {name}'s new fragment file."
                            );
#endif
                        }

                        return @$"<xml>
                                <fragments>{fragCount}</fragments>
                                <available>{isAvailable}</available>
                                </xml>";
                    }
                    case "getmsgnum":
                    {
                        if (File.Exists(playerProfilePath))
                        {
#if DEBUG
                            LoggerAccessor.LogInfo(
                                $"Existing {name}'s new fragment file detected, returning frag count num."
                            );
#endif
                            fragCount = Convert.ToInt32(File.ReadAllText(playerProfilePath));
                        }

                        return @$"<xml>
                                <fragments>{fragCount}</fragments>
                                <available>{isAvailable}</available>
                                </xml>";
                    }
                }
            }

            return null;
        }
    }
}
