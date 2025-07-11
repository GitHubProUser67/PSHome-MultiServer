using MultiServerLibrary.HTTP;
using System;

namespace WebAPIService.ILoveSony
{
    public class ILoveSonyClass
    {
        private string workpath;
        private string absolutepath;
        private string method;

        public ILoveSonyClass(string method, string absolutepath, string workpath)
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
                case "GET":
                    switch (absolutepath)
                    {
                        #region Resistance Fall of Man EULA
                        case "/i_love_sony/legal/UP9000-BCUS98107_00/1":
                            return MyResistanceEula.ILoveSonyEula();
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
