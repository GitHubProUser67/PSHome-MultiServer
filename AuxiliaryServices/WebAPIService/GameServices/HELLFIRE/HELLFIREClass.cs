using System;
using System.IO;
using System.Text;
using WebAPIService.GameServices.HELLFIRE.HFProcessors;

namespace WebAPIService.GameServices.HELLFIRE
{
    public class HELLFIREClass
    {
        private string workpath;
        private string absolutepath;
        private string method;

        public HELLFIREClass(string method, string absolutepath, string workpath)
        {
            this.absolutepath = absolutepath;
            this.workpath = workpath;
            this.method = method;
        }

        public byte[] ProcessRequest(byte[] PostData, string ContentType, bool https)
        {
            if (string.IsNullOrEmpty(absolutepath))
                return null;

            switch (method)
            {
                case "POST":
                    switch (absolutepath)
                    {
                        #region HomeTycoon
                        case "/HomeTycoon/Main_SCEE.php":
                            return Encoding.UTF8.GetBytes(TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https));
                        case "/HomeTycoon/Main_SCEJ.php":
                            return Encoding.UTF8.GetBytes(TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https));
                        case "/HomeTycoon/Main_SCEAsia.php":
                            return Encoding.UTF8.GetBytes(TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https));
                        case "/HomeTycoon/Main.php":
                            return Encoding.UTF8.GetBytes(TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https));
                        #endregion

                        #region ClearasilSkater
                        case "/ClearasilSkater/Main.php":
                            return Encoding.UTF8.GetBytes(ClearasilSkaterRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath));
                        #endregion

                        #region SlimJim Rescue
                        case "/SlimJim/Main.php":
                            return Encoding.UTF8.GetBytes(SlimJimRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath));
                        #endregion

                        #region Novus Primus Prime
                        case "/Main.php":
                            return Encoding.UTF8.GetBytes(NovusPrimeRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath));
                        #endregion

                        #region Poker
                        case "/PokerMain.php":
                        case "/DevPokerServer/PokerMain.php":
                        case "/PokerServer/PokerMain.php":
                            return Encoding.UTF8.GetBytes(PokerServerRequestProcessor.ProcessPokerMainPHP(PostData, ContentType, null, workpath));
                        #endregion
                        
                        default:    
                            break;
                    }
                    break;
                case "GET":
                    if (absolutepath.Contains("/Postcards/"))
                    {
                        string[] parts = Path.GetFileName(absolutepath).Split('.');

                        if (parts.Length == 3)
                        {
                            string jpgPostCardFilePath = $"{workpath}/HomeTycoon/TownsData/{parts[0]}/{parts[1]}.{parts[2]}";

                            if (File.Exists(jpgPostCardFilePath))
                                return File.ReadAllBytes(jpgPostCardFilePath);
                        }
                    }
                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
