using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using BlazeCommon;
using CastleLibrary.S0ny.XI5;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;

namespace MultiSocks.Blaze.Model
{
    public static class BlazeUser
    {
        public struct NETDATA
        {
            public uint IP;
            public uint PORT;
        }

        private static readonly UniqueIDGenerator _counter = new();

        public static ConcurrentDictionary<string, ConcurrentList<BlazeUserInfo>> AllPlayers =
            new();

        public static readonly object _sync = new();

        private const ulong MessengerPrefix = 0x7802000100000000;

        public class BlazeUserInfo
        {
            public struct SettingEntry
            {
                public string Key;
                public string Data;
            }

            private ulong _messengerId;

            public int ID;
            public long UserID = 0;
            public long PlayerID = 0;
            public string? AuthString;
            public string? Auth2String;
            public string? Name;
            public string? GameState;
            public string? CurrentGame;
            public XI5Ticket? PSNAuth;
            public string IP;
            public string PORT;
            public string SERVER_IP;
            public string SERVER_PORT;
            public string? pathtoprofile;
            public BlazeServerConnection BlazeClient;
            public ProtoFireConnection Client;
            public Stream ClientStream;
            public bool isActive = true;
            public bool Update = false;
            public bool SendOffers = false;
            public bool WaitsForJoining = false;
            public NETDATA EXIP;
            public NETDATA INIP;
            public Stopwatch PingTimer;

            public List<SettingEntry> Settings;
            public string timestring;

            public ulong MessengerId
            {
                get { return _messengerId; }
                set { _messengerId = MessengerPrefix | value; }
            }

            public BlazeUserInfo(
                BlazeServerConnection blazeClient,
                ProtoFireConnection client,
                Stream clientstream
            )
            {
                ID = (int)_counter.CreateSequentialID();
                BlazeClient = blazeClient;
                Client = client;
                ClientStream = clientstream;
                GameState = "boot";
                PingTimer = new Stopwatch();
                PingTimer.Start();
                IP = ((IPEndPoint?)Client.Socket.RemoteEndPoint)?.Address.ToString() ?? "127.0.0.1";
                PORT = ((IPEndPoint?)Client.Socket.RemoteEndPoint)?.Port.ToString() ?? "-1";
                SERVER_IP =
                    ((IPEndPoint?)Client.Socket.LocalEndPoint)?.Address.ToString()
                    ?? MultiSocksServerConfiguration.ServerBindAddress;
                SERVER_PORT =
                    ((IPEndPoint?)Client.Socket.LocalEndPoint)?.Address.ToString() ?? "-1";
                Settings = new List<SettingEntry>();
                timestring = string.Format(@"{0:yyyy-MM-dd_HHmmss}", DateTime.Now);
            }

            public uint GetRemoteIPvalue()
            {
                return InternetProtocolUtils.GetIPAddressAsUInt(IP);
            }

            public void UpdateSettings(string key, string data)
            {
                var found = false;
                var newset = new SettingEntry { Key = key, Data = data };
                lock (_sync)
                {
                    for (var i = 0; i < Settings.Count; i++)
                        if (Settings[i].Key == key)
                        {
                            Settings[i] = newset;
                            found = true;
                            break;
                        }
                    if (!found)
                        Settings.Add(newset);
                    if (!string.IsNullOrEmpty(pathtoprofile))
                    {
                        var result = new List<string>();
                        var lines = File.ReadAllLines(pathtoprofile);
                        for (var i = 0; i < 5; i++)
                            result.Add(lines[i]);
                        foreach (var set in Settings)
                            result.Add(set.Key + "=" + set.Data);
                        File.WriteAllLines(pathtoprofile, result.ToArray());
                    }
                    Update = true;
                }
            }

            public string GetSettingPerKey(string key)
            {
                lock (_sync)
                    return Settings.FirstOrDefault(x => x.Key == key).Data ?? string.Empty;
            }

            public string GetSettings()
            {
                lock (_sync)
                {
                    var res = string.Empty;
                    foreach (var set in Settings)
                        res += "  " + set.Key + " = " + set.Data + "\n";
                    return res;
                }
            }

            public void SetJoinWaitState(bool state)
            {
                WaitsForJoining = state;
            }

            public void SetActiveState(bool state)
            {
                isActive = state;
            }

            public static BlazeUserInfo? GetServerUserByUserID(string srvIdent, long userId)
            {
                if (!AllPlayers.TryGetValue(srvIdent, out var value))
                    return null;

                foreach (var user in value)
                {
                    if (user.UserID == userId)
                        return user;
                }

                return null; // not found
            }

            public static BlazeUserInfo? GetServerUserByMessengerID(
                string srvIdent,
                ulong messengerId
            )
            {
                if (!AllPlayers.TryGetValue(srvIdent, out var value))
                    return null;

                foreach (var user in value)
                {
                    if (user.MessengerId == messengerId)
                        return user;
                }

                return null; // not found
            }

            public static BlazeUserInfo? GetUser(BlazeServerConnection blazeServerConnection)
            {
                return AllPlayers
                    .Values.SelectMany(playerList => playerList)
                    .FirstOrDefault(user => user.BlazeClient == blazeServerConnection);
            }

            public static BlazeUserInfo? GetUser(ProtoFireConnection protoFireConnection)
            {
                return AllPlayers
                    .Values.SelectMany(playerList => playerList)
                    .FirstOrDefault(user => user.Client == protoFireConnection);
            }

            public static bool RemoveServerUserByUserID(string srvIdent, long userId)
            {
                if (!AllPlayers.TryGetValue(srvIdent, out var users))
                    return false;

                foreach (var user in users)
                {
                    if (user.UserID == userId)
                        return users.Remove(user);
                }

                return false; // not found
            }

            public static bool RemoveServerUserByMessengerID(string srvIdent, ulong messengerId)
            {
                if (!AllPlayers.TryGetValue(srvIdent, out var users))
                    return false;

                foreach (var user in users)
                {
                    if (user.MessengerId == messengerId)
                        return users.Remove(user);
                }

                return false; // not found
            }

            public static bool RemoveUser(BlazeServerConnection blazeServerConnection)
            {
                foreach (var users in AllPlayers.Values)
                {
                    BlazeUserInfo? toRemove = null;

                    foreach (var user in users)
                    {
                        if (user.BlazeClient == blazeServerConnection)
                        {
                            toRemove = user;
                            break;
                        }
                    }

                    if (toRemove != null)
                        return users.Remove(toRemove);
                }

                return false;
            }

            public static bool RemoveUser(ProtoFireConnection protoFireConnection)
            {
                foreach (var users in AllPlayers.Values)
                {
                    BlazeUserInfo? toRemove = null;

                    foreach (var user in users)
                    {
                        if (user.Client == protoFireConnection)
                        {
                            toRemove = user;
                            break;
                        }
                    }

                    if (toRemove != null)
                        return users.Remove(toRemove);
                }

                return false;
            }
        }
    }
}
