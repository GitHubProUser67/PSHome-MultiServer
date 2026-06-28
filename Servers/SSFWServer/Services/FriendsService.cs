using System.Text;
using CustomLogger;

namespace SSFWServer.Services
{
    public class FriendsService(string sessionid, string env, string? key)
    {
        private readonly string? sessionid = sessionid;
        private readonly string? env = env;
        private readonly string? key = key;

        public string HandleFriendsService(string absolutepath, byte[] buffer)
        {
            var userName = SSFWUserSessionManager.GetIdBySessionId(sessionid);
            var friendsStorePath =
                $"{SSFWServerConfiguration.SSFWStaticFolder}/FriendsService/{env}";
            try
            {
                Directory.CreateDirectory(friendsStorePath);

                File.WriteAllText(
                    $"{friendsStorePath}/{userName}.txt",
                    Encoding.UTF8.GetString(buffer)
                );
#if DEBUG
                LoggerAccessor.LogInfo(
                    $"[SSFW] FriendsService - HandleFriendsService Friends list posted: {userName} at {$"{friendsStorePath}/{userName}.txt"}"
                );
#endif
                return "Success";
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[SSFW] FriendsService - HandleFriendsService ERROR caught: \n{ex}"
                );
                return ex.Message;
            }
        }
    }
}
