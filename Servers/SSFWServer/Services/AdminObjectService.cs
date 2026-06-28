using CustomLogger;
using Newtonsoft.Json;
using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class AdminObjectService(string sessionid, string? key)
    {
        private readonly string? sessionid = sessionid;
        private readonly string? key = key;

        public bool HandleAdminObjectService(string UserAgent)
        {
            return IsAdminVerified(UserAgent);
        }

        //Helper function for other uses in SSFW services
        public bool IsAdminVerified(string userAgent)
        {
            var userName = SSFWUserSessionManager.GetUsernameBySessionId(sessionid);
            var accountFilePath =
                $"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{userName}.json";

            if (!string.IsNullOrEmpty(userName) && File.Exists(accountFilePath))
            {
                var userprofiledata = FileHelper.ReadAllText(accountFilePath, key);

                if (!string.IsNullOrEmpty(userprofiledata))
                {
                    // Parsing JSON data to SSFWUserData object
                    var userData = JsonConvert.DeserializeObject<SSFWUserData>(userprofiledata);

                    if (userData != null)
                    {
                        LoggerAccessor.LogInfo(
                            $"[SSFW] - IsAdminVerified : IGA Request from : {userAgent}/{userName} - IGA status : {userData.IGA}"
                        );

                        if (userData.IGA == 1)
                        {
                            LoggerAccessor.LogInfo(
                                $"[SSFW] - IsAdminVerified : Admin role confirmed for : {userAgent}/{userName}"
                            );

                            return true;
                        }
                    }
                }
            }

            LoggerAccessor.LogError(
                $"[SSFW] - IsAdminVerified : IGA Access denied for {userAgent}!"
            );

            return false;
        }
    }
}
