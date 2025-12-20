using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
{
    [RMCService((ushort)RMCProtocolId.LocalizationService)]
    public class LocalizationService : RMCServiceBase
    {
        private static string LocaleCode = "en-US";

        [RMCMethod(1)]
        public RMCResult GetLocaleCode()
        {
            return Result(LocaleCode);
        }

        [RMCMethod(2)]
        public RMCResult SetLocaleCode(string localeCode)
        {
            LocaleCode = localeCode;
            return Error(0);
        }
    }
}
