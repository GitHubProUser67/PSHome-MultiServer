using System;
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

        public string ProcessRequest(byte[] PostData, string ContentType, bool https)
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
                            return TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https);
                        case "/HomeTycoon/Main_SCEJ.php":
                            return TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https);
                        case "/HomeTycoon/Main_SCEAsia.php":
                            return TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https);
                        case "/HomeTycoon/Main.php":
                            return TycoonRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath, https);
                        #endregion

                        #region ClearasilSkater
                        case "/ClearasilSkater/Main.php":
                            return ClearasilSkaterRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath);
                        #endregion

                        #region SlimJim Rescue
                        case "/SlimJim/Main.php":
                            return SlimJimRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath);
                        #endregion

                        #region Novus Primus Prime
                        case "/Main.php":
                            return NovusPrimeRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath);
                        #endregion

                        #region 
                        case "/PokerMain.php":
                        case "/DevPokerServer/PokerMain.php":
                        case "/PokerServer/PokerMain.php":
                            return PokerServerRequestProcessor.ProcessPokerMainPHP(PostData, ContentType, null, workpath);
                        #endregion

                        #region Giftinator
                        case "/Giftinator/Main.php":
                            //return GiftinatorRequestProcessor.ProcessMainPHP(PostData, ContentType, null, workpath);
                        #endregion
                        
                        default:    
                            break;
                    }
                    break;
                default:
                    break;
            }

            return null;
        }
    }
}
