using CustomLogger;

namespace SSFWServer.Services
{
    public class AchievementService
    {
        private string? sessionid;
        private string? env;
        private string? key;

        public AchievementService(string sessionid, string env, string? key)
        {
            this.sessionid = sessionid;
            this.env = env;
            this.key = key;
        }

        public string HandleAchievementService(string absolutePath)
        {
            string? userName = SSFWUserSessionManager.GetUsernameBySessionId(sessionid);
#if DEBUG
            LoggerAccessor.LogInfo($"[SSFW] AchievementService - Requesting {userName}'s achievements");
#endif
            //We send empty response as status 200 for now
            return $"{{}}";
        }
    }
}
