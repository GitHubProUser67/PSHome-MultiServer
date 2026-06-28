using CustomLogger;

namespace SSFWServer.Services
{
    public class AchievementService(string sessionid, string env, string? key)
    {
        private readonly string? sessionid = sessionid;
        private readonly string? env = env;
        private readonly string? key = key;

        public string HandleAchievementService(string absolutePath)
        {
            var userName = SSFWUserSessionManager.GetUsernameBySessionId(sessionid);
#if DEBUG
            LoggerAccessor.LogInfo(
                $"[SSFW] AchievementService - Requesting {userName}'s achievements"
            );
#endif
            //We send empty response as status 200 for now
            return $"{{}}";
        }
    }
}
