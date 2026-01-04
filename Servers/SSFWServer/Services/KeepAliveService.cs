using SSFWServer.Helpers.RegexHelper;

namespace SSFWServer.Services
{
    public class KeepAliveService
    {
        public static bool UpdateKeepAliveForClient(string absolutePath)
        {
            string resultSessionId = absolutePath.Split("/")[3];
            if (GUIDValidator.RegexSessionValidator.IsMatch(resultSessionId))
                return SSFWUserSessionManager.UpdateKeepAliveTime(resultSessionId);
            return false;
        }
    }
}