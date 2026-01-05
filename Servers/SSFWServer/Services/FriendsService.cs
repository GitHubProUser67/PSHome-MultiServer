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
            string? userName = SSFWUserSessionManager.GetIdBySessionId(sessionid);
            string friendsStorePath = $"{SSFWServerConfiguration.SSFWStaticFolder}/FriendsService/{env}";
            try
            {
                Directory.CreateDirectory(friendsStorePath);

                File.WriteAllText($"{friendsStorePath}/{userName}.txt", Encoding.UTF8.GetString(buffer));
#if DEBUG
                LoggerAccessor.LogInfo($"[SSFW] FriendsService - HandleFriendsService Friends list posted: {userName} at {$"{friendsStorePath}/{userName}.txt"}");
#endif
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