using CustomLogger;
using System.Text;

namespace SSFWServer.Services
{
    public class FriendsService
    {
        private string? sessionid;
        private string? env;
        private string? key;

        public FriendsService(string sessionid, string env, string? key)
        {
            this.sessionid = sessionid;
            this.env = env;
            this.key = key;
        }

        public string HandleFriendsService(string absolutepath, byte[] buffer)
        {
            string? userName = SSFWUserSessionManager.GetUsernameBySessionId(sessionid);
            string auditLogPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}/{env}";
            try
            {
                Directory.CreateDirectory(auditLogPath);

                File.WriteAllText($"{auditLogPath}/{userName}.txt", Encoding.UTF8.GetString(buffer));
                LoggerAccessor.LogInfo($"[SSFW] FriendsService - HandleFriendsService Friends list posted: {userName}");
                return "Success";
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SSFW] FriendsService - HandleFriendsService ERROR caught: \n{ex}");
                return ex.Message;
            }
        }
    }
}