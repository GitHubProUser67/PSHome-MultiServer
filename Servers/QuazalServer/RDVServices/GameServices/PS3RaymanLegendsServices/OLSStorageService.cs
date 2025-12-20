using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3RaymanLegendsServices
{
    [RMCService((ushort)RMCProtocolId.OLSStorageService)]
    public class OLSStorageService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult LoadVersion()
        {
            ushort version = 1;
            string sandboxName = "RaymanPS3";
            uint applicationMask = 1;
            return Result(new { version, sandboxName, applicationMask });
        }

        [RMCMethod(2)]
        public RMCResult SaveLocale(string localeCode, string playerName)
        {
            return Error(0);
        }
    }
}
