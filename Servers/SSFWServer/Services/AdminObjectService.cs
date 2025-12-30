using CustomLogger;
using Newtonsoft.Json;
using SSFWServer.Helpers;
using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class AdminObjectService
    {
        private string? sessionid;
        private string? key;

        public AdminObjectService(string sessionid, string? key)
        {
            this.sessionid = sessionid;
            this.key = key;
        }

        public bool HandleAdminObjectService(string UserAgent)
        {
            return IsAdminVerified(UserAgent);
        }

        //Helper function for other uses in SSFW services
        public bool IsAdminVerified(string userAgent)
        {
            string? userName = SSFWUserSessionManager.GetUsernameBySessionId(sessionid);
            string accountFilePath = $"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{userName}.json";

            if (!string.IsNullOrEmpty(userName) && File.Exists(accountFilePath))
            {
                string? userprofiledata = FileHelper.ReadAllText(accountFilePath, key);

                if (!string.IsNullOrEmpty(userprofiledata))
                {
                    // Parsing JSON data to SSFWUserData object
                    SSFWUserData? userData = JsonConvert.DeserializeObject<SSFWUserData>(userprofiledata);

                    if (userData != null)
                    {
                        LoggerAccessor.LogInfo($"[SSFW] - IsAdminVerified : IGA Request from : {userAgent}/{userName} - IGA status : {userData.IGA}");

                        if (userData.IGA == 1)
                        {
                            LoggerAccessor.LogInfo($"[SSFW] - IsAdminVerified : Admin role confirmed for : {userAgent}/{userName}");

                            return true;
                        }
                    }
                }
            }

            LoggerAccessor.LogError($"[SSFW] - IsAdminVerified : IGA Access denied for {userAgent}!");

            return false;
        }
    }
}
