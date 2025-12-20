using CustomLogger;
using Newtonsoft.Json;
using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class SSFWAdminObjectService
    {
        private string? sessionid;
        private string? key;

        public SSFWAdminObjectService(string sessionid, string? key)
        {
            this.sessionid = sessionid;
            this.key = key;
        }

        public bool HandleAdminObjectService(string UserAgent)
        {
            string? username = SSFWUserSessionManager.GetUsernameBySessionId(sessionid);

            if (!string.IsNullOrEmpty(username) && File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{username}.json"))
            {
                string? userprofiledata = FileHelper.ReadAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{username}.json", key);

                if (!string.IsNullOrEmpty(userprofiledata))
                {
                    // Parsing JSON data to SSFWUserData object
                    SSFWUserData? userData = JsonConvert.DeserializeObject<SSFWUserData>(userprofiledata);

                    if (userData != null)
                    {
                        LoggerAccessor.LogInfo($"[SSFW] - IGA Request from : {UserAgent}/{username} - IGA status : {userData.IGA}");

                        if (userData.IGA == 1)
                        {
                            LoggerAccessor.LogInfo($"[SSFW] - Admin role confirmed for : {UserAgent}/{username}");

                            return true;
                        }
                    }
                }
            }

            LoggerAccessor.LogError($"[SSFW] - {UserAgent} requested IGA access, but no access allowed so we forbid!");

            return false;
        }
    }
}
