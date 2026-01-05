using CustomLogger;
using SSFWServer.Helpers.RegexHelper;
using System.Collections.Concurrent;

namespace SSFWServer
{
    public class SSFWUserSessionManager
    {
        private static ConcurrentDictionary<string, (int, UserSession, DateTime)> userSessions = new();

        public static void RegisterUser(string userName, string sessionid, string id, int realuserNameSize)
        {
            if (userSessions.TryGetValue(sessionid, out (int, UserSession, DateTime) sessionEntry))
                UpdateKeepAliveTime(sessionid, sessionEntry);
            else if (userSessions.TryAdd(sessionid, (realuserNameSize, new UserSession { Username = userName, Id = id }, DateTime.Now.AddMinutes(SSFWServerConfiguration.SSFWTTL))))
                LoggerAccessor.LogInfo($"[UserSessionManager] - User '{userName}' successfully registered with SessionId '{sessionid}'.");
            else
                LoggerAccessor.LogError($"[UserSessionManager] - Failed to register User '{userName}' with SessionId '{sessionid}'.");
        }

        public static string? GetSessionIdByUsername(string? userName, bool rpcn)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            foreach (var kvp in userSessions)
            {
                string sessionId = kvp.Key;
                var (realSize, session, _) = kvp.Value;

                string? realUsername = session.Username?.Substring(0, realSize);

                if (string.Equals(realUsername + (rpcn ? "@RPCN" : string.Empty), userName, StringComparison.Ordinal))
                    return sessionId;
            }

            return null;
        }

        public static string? GetUsernameBySessionId(string? sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return null;

            if (userSessions.TryGetValue(sessionId, out (int, UserSession, DateTime) sessionEntry))
                return sessionEntry.Item2.Username;

            return null;
        }

        public static string? GetFormatedUsernameBySessionId(string? sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return null;

            if (userSessions.TryGetValue(sessionId, out (int, UserSession, DateTime) sessionEntry))
            {
                string? userName = sessionEntry.Item2.Username;

                if (!string.IsNullOrEmpty(userName) && userName.Length > sessionEntry.Item1)
                    userName = userName.Substring(0, sessionEntry.Item1);

                return userName;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the Id of a user by their Username, if they have an active session.
        /// </summary>
        /// <param name="userName">The username to search for (case-sensitive).</param>
        /// <returns>The user's Id if found and session is active, otherwise null.</returns>
        public static string? GetIdByUsername(string? userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            foreach (var entry in userSessions.Values)
            {
                if (!string.IsNullOrEmpty(entry.Item2.Username))

                    // Check if session is still valid and username matches
                    if (entry.Item3 > DateTime.Now
                        && entry.Item2.Username.StartsWith(userName)
                        && HasMinimumClientVersion(userName))
                    {
                        return entry.Item2.Id;
                    }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the full UserSession object by Username, if they have an active session.
        /// </summary>
        /// <param name="userName">The username to search for (case-sensitive).</param>
        /// <returns>The UserSession if found and active, otherwise null.</returns>
        public static UserSession? GetUserSessionByUsername(string? userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            foreach (var entry in userSessions.Values)
            {
                if (!string.IsNullOrEmpty(entry.Item2.Username))

                    if (entry.Item3 > DateTime.Now
                        && entry.Item2.Username.StartsWith(userName)
                        && HasMinimumClientVersion(userName))
                    {
                        return entry.Item2;
                    }
            }

            return null;
        }


        public static string? GetIdBySessionId(string? sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                return null;

            (bool, string?) sessionTuple = IsSessionValid(sessionId, false);

            if (sessionTuple.Item1)
                return sessionTuple.Item2;

            return null;
        }

        public static bool UpdateKeepAliveTime(string sessionid, (int, UserSession, DateTime) sessionEntry = default)
        {
            if (sessionEntry == default)
            {
                if (!userSessions.TryGetValue(sessionid, out sessionEntry))
                    return false;
            }

            DateTime KeepAliveTime = DateTime.Now.AddMinutes(SSFWServerConfiguration.SSFWTTL);

            sessionEntry.Item3 = KeepAliveTime;

            if (userSessions.ContainsKey(sessionid))
            {
                LoggerAccessor.LogInfo($"[SSFWUserSessionManager] - Updating: {sessionEntry.Item2?.Username} session with id: {sessionEntry.Item2?.Id} keep-alive time to:{KeepAliveTime}.");
                userSessions[sessionid] = sessionEntry;
                return true;
            }

            LoggerAccessor.LogError($"[SSFWUserSessionManager] - Failed to update: {sessionEntry.Item2?.Username} session with id: {sessionEntry.Item2?.Id} keep-alive time.");
            return false;
        }


        public static (bool, string?) IsSessionValid(string? sessionId, bool cleanupDeadSessions)
        {
            if (string.IsNullOrEmpty(sessionId))
                return (false, null);

            if (userSessions.TryGetValue(sessionId, out (int, UserSession, DateTime) sessionEntry))
            {
                if (sessionEntry.Item3 > DateTime.Now)
                    return (true, sessionEntry.Item2.Id);
                else if (cleanupDeadSessions)
                {
                    // Clean up expired entry.
                    if (userSessions.TryRemove(sessionId, out sessionEntry))
                        LoggerAccessor.LogWarn($"[SSFWUserSessionManager] - Cleaned: {sessionEntry.Item2.Username} session with id: {sessionEntry.Item2.Id}...");
                    else
                        LoggerAccessor.LogError($"[SSFWUserSessionManager] - Failed to clean: {sessionEntry.Item2.Username} session with id: {sessionEntry.Item2.Id}...");
                }
            }

            return (false, null);
        }

        public static void SessionCleanupLoop(object? state)
        {
            lock (userSessions)
            {
                foreach (var sessionId in userSessions.Keys)
                {
                    IsSessionValid(sessionId, true);
                }
            }
        }

        private static bool HasMinimumClientVersion(string username, int minimumVersion = 016531)
        {
            var match = GUIDValidator.VersionFilter.Match(username);
            if (match.Success && int.TryParse(match.Value, out int version))
            {
                return version >= minimumVersion;
            }
            return false;
        }
    }

    public class UserSession
    {
        public string? Username { get; set; }
        public string? Id { get; set; }
    }

}