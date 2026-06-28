using SSFWServer.Helpers.RegexHelper;

namespace SSFWServer.Services
{
    public class KeepAliveService
    {
        public static bool UpdateKeepAliveForClient(string absolutePath)
        {
            var resultSessionId = absolutePath.Split("/")[3];
            return GUIDValidator.RegexSessionValidator.IsMatch(resultSessionId)
                && SSFWUserSessionManager.UpdateKeepAliveTime(resultSessionId);
        }
    }
}
