using System;
using System.Collections.Generic;
using System.IO;

namespace WebAPIService.GameServices.UBISOFT.gsconnect
{
    public class gsconnectClass
    {
        private string absolutepath;
        private string method;
        private string apistaticpath;

        public gsconnectClass(string method, string absolutepath, string apiStaticpath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
            apistaticpath = apiStaticpath;
        }

        public (string, string, Dictionary<string, string>) ProcessRequest(IDictionary<string, string> QueryParameters, byte[] PostData = null, string ContentType = null)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return (null, null, null);

            apistaticpath += "/UBISOFT/gsconnect/";

            switch (method)
            {
                case "GET":
                    switch (absolutepath)
                    {
                        case "/gsinit.php":
                            if (QueryParameters != null && QueryParameters.ContainsKey("dp") && QueryParameters.ContainsKey("user"))
                            {
                                string ini_file = null;
                                string dp = QueryParameters["dp"];
                                string user = QueryParameters["user"];

                                switch (dp)
                                {
                                    case "HEROES_657d2c2ebadc6a1d":
                                        ini_file = "homm5/servers.ini";
                                        break;
                                    case "GHOSTRECONIT_PS2":
                                        ini_file = "grjs/GS.ini";
                                        break;
                                    case "SPLINTERCELL3PS2US":
                                    case "SPLINTERCELL3PC":
                                    case "SPLINTERCELL3PCCOOP":
                                    case "SPLINTERCELL3PCADVERS":
                                        ini_file = "sp3/GS.ini";
                                        break;
                                    default:
                                        CustomLogger.LoggerAccessor.LogWarn($"[gsconnectClass] - Unknown game in gsinit.php: {dp}");
                                        break;
                                }

                                if (!string.IsNullOrEmpty(ini_file))
                                {
                                    string filePath = apistaticpath + ini_file;
                                    if (File.Exists(filePath))
                                        return (File.ReadAllText(filePath), "application/octet-stream", new Dictionary<string, string> { { "Content-Disposition", $"attachment; filename={Path.GetFileName(filePath)}" } });
                                    else
                                        CustomLogger.LoggerAccessor.LogWarn($"[gsconnectClass] - game: {dp} requested a non-existant file, path: {filePath}");
                                }
                            }
                            else
                                CustomLogger.LoggerAccessor.LogWarn($"[gsconnectClass] - gsinit.php was requested with wrong parameters!");
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return (null, null, null);
        }
    }
}
