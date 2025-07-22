using System.IO;
namespace WebAPIService.GameServices.UBISOFT.HERMES_API.v2
{
    public class V2ConfigurationClass
    {
        public static (string, string) HandleConfigurationGET(string apipath, string UbiAppId)
        {
            if (File.Exists(apipath + $"/UBISOFT/HERMES/v2/configuration/{UbiAppId}.json"))
                return (File.ReadAllText(apipath + $"/UBISOFT/HERMES/v2/configuration/{UbiAppId}.json"), "application/json; charset=utf-8");
            else
                CustomLogger.LoggerAccessor.LogWarn($"[HERMES] - Unknown configuration requested with AppId {UbiAppId}");

            return (null, null);
        }
    }
}
