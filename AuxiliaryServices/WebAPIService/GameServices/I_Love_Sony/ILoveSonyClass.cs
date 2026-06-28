namespace WebAPIService.GameServices.I_Love_Sony
{
    public class ILoveSonyClass(string method, string absolutepath, string workpath)
    {
        private readonly string workpath = workpath;
        private readonly string absolutepath = absolutepath;
        private readonly string method = method;

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
