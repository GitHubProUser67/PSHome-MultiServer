using CustomLogger;

namespace SSFWServer.Services
{
    public class PlayerLookupService
    {
        public static string HandlePlayerLookupService(string url)
        {
            var byDisplayName = url.Split("=")[1];
            var userId = SSFWUserSessionManager.GetIdByUsername(byDisplayName);
#if DEBUG
            LoggerAccessor.LogInfo(
                $"[SSFW] PlayerLookupService - Requesting {byDisplayName}'s id, successfully returned userId {userId}"
            );
#endif
            return $"{{\"@id\": {userId} }}";
        }
    }
}
