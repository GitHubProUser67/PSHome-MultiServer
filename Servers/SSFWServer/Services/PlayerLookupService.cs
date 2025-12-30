using CustomLogger;

namespace SSFWServer.Services
{
    public class PlayerLookupService
    {
        public string HandlePlayerLookupService(string url)
        {
            string byDisplayName = url.Split("=")[1];
            string? userId = SSFWUserSessionManager.GetIdByUsername(byDisplayName);
#if DEBUG
            LoggerAccessor.LogInfo($"[SSFW] PlayerLookupService - Requesting {byDisplayName}'s id, successfully returned userId {userId}");
#endif
            return $"{{\"@id\": {userId} }}";
        }
    }
}