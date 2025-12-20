using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using System.IO;
using System;
using System.Xml.Serialization;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.Xi2
{
    public class ArticlesCont
    {
        public static string ProcessArticlesCont(DateTime CurrentDate, byte[] PostData, string ContentType, string apipath)
        {
            string func = null;
            string key = null;
            string ExpectedHash = null;
            string name = null;
            string email = null;
            string hash = null;
            string articles = null;
            string videos = null;
            string avatars = null;
            string quinta = null;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    func = data.GetParameterValue("func");
                    key = data.GetParameterValue("key");
                    name = data.GetParameterValue("name");

                    if (!string.IsNullOrEmpty(func))
                    {
                        string directoryPath = apipath + $"/NDREAMS/Xi2/PlayersInventory/{name}";
                        string profilePath = directoryPath + "/ArticlesCont.xml";

                        switch (func)
                        {
                            case "init":
                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    ArticlesContProfileData profileData;

                                    if (File.Exists(profilePath))
                                        profileData = ArticlesContProfileData.DeserializeProfileData(profilePath);
                                    else
                                    {
                                        profileData = new ArticlesContProfileData() { Email = string.Empty, Articles = string.Empty, Videos = string.Empty, Avatars = string.Empty, Quinta = string.Empty };

                                        Directory.CreateDirectory(directoryPath);
                                        profileData.SerializeProfileData(profilePath);
                                    }

                                    return $"<xml><success>true</success><result><email>{profileData.Email}</email><docs>{profileData.Articles}</docs><videos>{profileData.Videos}</videos><avatars>{profileData.Avatars}</avatars>" +
                                        $"<quinta>{profileData.Quinta}</quinta><confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{profileData.Avatars}{profileData.Email}{profileData.Articles}{profileData.Quinta}{profileData.Videos}", CurrentDate)}" +
                                        $"</confirm></result></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - ArticlesCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                            case "update_email":
                                email = data.GetParameterValue("email");
                                hash = data.GetParameterValue("hash");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func + email, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ArticlesContProfileData profileData = ArticlesContProfileData.DeserializeProfileData(profilePath);
                                        profileData.Email = email;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Hash>{hash}</Hash><email>{profileData.Email}</email>" +
                                            $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{hash}{profileData.Email}", CurrentDate)}" +
                                            $"</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - ArticlesCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - ArticlesCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                            case "update_articles":
                                hash = data.GetParameterValue("hash");
                                articles = data.GetParameterValue("articles");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func + articles, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ArticlesContProfileData profileData = ArticlesContProfileData.DeserializeProfileData(profilePath);
                                        profileData.Articles = articles;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Hash>{hash}</Hash><docs>{profileData.Articles}</docs>" +
                                            $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{hash}{profileData.Articles}", CurrentDate)}" +
                                            $"</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - ArticlesCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - ArticlesCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                            case "update_videos":
                                hash = data.GetParameterValue("hash");
                                videos = data.GetParameterValue("videos");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func + videos, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ArticlesContProfileData profileData = ArticlesContProfileData.DeserializeProfileData(profilePath);
                                        profileData.Videos = videos;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Hash>{hash}</Hash><videos>{profileData.Videos}</videos>" +
                                            $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{hash}{profileData.Videos}", CurrentDate)}" +
                                            $"</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - ArticlesCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - ArticlesCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                            case "update_avatars":
                                hash = data.GetParameterValue("hash");
                                avatars = data.GetParameterValue("avatars");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func + avatars, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ArticlesContProfileData profileData = ArticlesContProfileData.DeserializeProfileData(profilePath);
                                        profileData.Avatars = avatars;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Hash>{hash}</Hash><avatars>{profileData.Avatars}</avatars>" +
                                            $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{hash}{profileData.Avatars}", CurrentDate)}" +
                                            $"</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - ArticlesCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - ArticlesCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                            case "update_quinta":
                                hash = data.GetParameterValue("hash");
                                quinta = data.GetParameterValue("quinta");

                                ExpectedHash = NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, func + quinta, CurrentDate);

                                if (ExpectedHash.Equals(key))
                                {
                                    if (File.Exists(profilePath))
                                    {
                                        ArticlesContProfileData profileData = ArticlesContProfileData.DeserializeProfileData(profilePath);
                                        profileData.Quinta = quinta;
                                        profileData.SerializeProfileData(profilePath);

                                        return $"<xml><success>true</success><result><Success>true</Success><Hash>{hash}</Hash><quinta>{profileData.Quinta}</quinta>" +
                                            $"<confirm>{NDREAMSServerUtils.Server_GetSignatureCustom(Cont.ContSignature, name, $"{hash}{profileData.Quinta}", CurrentDate)}" +
                                            $"</confirm></result></xml>";
                                    }

                                    string errMsg = $"[Xi2] - ArticlesCont: Profile doesn't exist!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>No Profile available</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[Xi2] - ArticlesCont: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><success>false</success><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessArticlesCont</function></xml>";
                                }
                        }
                    }

                    ms.Flush();
                }
            }

            return null;
        }
    }

    public class ArticlesContProfileData
    {
        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "Articles")]
        public string Articles { get; set; }

        [XmlElement(ElementName = "Videos")]
        public string Videos { get; set; }

        [XmlElement(ElementName = "Avatars")]
        public string Avatars { get; set; }

        [XmlElement(ElementName = "Quinta")]
        public string Quinta { get; set; }

        public void SerializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ArticlesContProfileData));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static ArticlesContProfileData DeserializeProfileData(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ArticlesContProfileData));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (ArticlesContProfileData)serializer.Deserialize(reader);
            }
        }
    }
}
