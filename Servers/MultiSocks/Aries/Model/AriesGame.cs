using System.Collections.Concurrent;
using MultiServerLibrary.Extension;
using MultiSocks.Aries.Components;

namespace MultiSocks.Aries.Model
{
    public class AriesGame
    {
        public int MaxSize;
        public int MinSize;
        public int ID;
        public int RoomID;
        public string Ident;
        public string? SKU;
        public string SysFlags;
        public string? CustFlags;
        public string Params;
        public string Name;
        public string? Seed;
        public string? pass;
        public AriesUser? Host;
        public AriesUser? GPSHost;
        public bool Priv;
        public bool Started = false;

        public UserCollection Users = new();
        private readonly List<AriesUser> UsersCache = new(); // This is necessary to prevent users leaving during a ranked event.
        private readonly ConcurrentDictionary<int, bool> _pIdIsUsed = new();

        private readonly object _ClientIndexlock = new();

        public AriesGame(
            int maxSize,
            int minSize,
            int id,
            string ident,
            string? sku,
            string? custFlags,
            string @params,
            string name,
            bool priv,
            string? seed,
            string sysFlags,
            string? Pass,
            int roomId
        )
        {
            MaxSize = maxSize;
            MinSize = minSize;
            ID = id;
            SKU = sku;
            Ident = ident;
            CustFlags = custFlags;
            Params = @params;
            Name = name;
            Priv = priv;
            Seed = seed;
            SysFlags = sysFlags;
            pass = Pass;
            RoomID = roomId;

            // populate collection of used player ids
            for (var i = 0; i < maxSize; ++i)
                _pIdIsUsed.TryAdd(i, false);
        }

        public int GetActiveUsersCount()
        {
            return Users.Count() + (Host != null ? 1 : 0);
        }

        private bool TryRegisterNewClientIndex(out int index)
        {
            lock (_ClientIndexlock)
            {
                for (index = 0; index < _pIdIsUsed.Count; ++index)
                {
                    if (_pIdIsUsed.TryGetValue(index, out var isUsed) && !isUsed)
                    {
                        _pIdIsUsed[index] = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public void UnregisterClientIndex(int index)
        {
            _pIdIsUsed[index] = false;
        }

        // true as a return value means close game.
        public bool RemoveUserAndCheckGameValidity(
            AriesUser user,
            int reason = 0,
            string? KickReason = ""
        )
        {
            lock (Users)
            {
                Users.RemoveUser(user);
                if (user.CurrentGameIndex != -1)
                {
                    UnregisterClientIndex(user.CurrentGameIndex);
                    user.CurrentGameIndex = -1;
                }

                user.CurrentGame = null;

                var minSizeAllowed = MinSize;

                if (Ident.Contains("NASCAR09") && "PS3".Equals(SKU))
                    minSizeAllowed--; // Nascar next-gen has an extra player in MinSize, but only to tell server not to start the game under 3 players.

                if (GetActiveUsersCount() < minSizeAllowed || GPSHost == user) // End Game.
                {
                    user.SendPlusWho(user, user.Connection?.Context.Project);

                    foreach (var batchuser in Users.GetAll())
                    {
                        Users.RemoveUser(batchuser);
                        if (user.CurrentGameIndex != -1)
                        {
                            UnregisterClientIndex(user.CurrentGameIndex);
                            user.CurrentGameIndex = -1;
                        }

                        batchuser.CurrentGame = null;

                        batchuser.SendPlusWho(batchuser, batchuser.Connection?.Context.Project);
                    }

                    return true;
                }
                else
                {
                    if (reason == 1)
                        user.Connection?.SendMessage(new PlusKik() { REASON = KickReason }); // Thank you Bo98!
                    else
                        user.SendPlusWho(user, user.Connection?.Context.Project);

                    if (user.Connection?.Context is MatchmakerServer mc)
                        BroadcastPopulation(mc);
                }
            }

            return false;
        }

        public void AddHost(AriesUser? user)
        {
            if (user == null)
                return;

            Host = user;
        }

        public void AddGPSHost(AriesUser? user)
        {
            if (user == null)
                return;

            TryRegisterNewClientIndex(out user.CurrentGameIndex);
            Users.AddUser(user);
            GPSHost = user;
            Host ??= user;
        }

        public void AddUser(AriesUser? user)
        {
            if (user == null)
                return;

            TryRegisterNewClientIndex(out user.CurrentGameIndex);
            Users.AddUser(user);
        }

        public bool RemovePlayerByUsername(
            string? username,
            int reason = 0,
            string? KickReason = ""
        )
        {
            if (string.IsNullOrEmpty(username))
                return false;

            var userToRemove = Users.GetUserByName(username);

            return userToRemove != null
                && RemoveUserAndCheckGameValidity(userToRemove, reason, KickReason);
        }

        public void SetGameStatus(bool status)
        {
            Started = status;

            // Clear the user list (except the host) and build it back.
            lock (UsersCache)
            {
                UsersCache.Clear();
                if (status)
                    UsersCache.AddRange(Users.GetAll());
            }
        }

        public GenericMessage GetGameDetails(string msg)
        {
            Dictionary<string, string?> OutputCache = new()
            {
                { "IDENT", ID.ToString() },
                { "HOST", '@' + Host?.Username },
                { "NAME", Name },
                { "ROOM", RoomID.ToString() },
                { "MAXSIZE", MaxSize.ToString() },
                { "MINSIZE", MinSize.ToString() },
                { "COUNT", GetActiveUsersCount().ToString() },
                { "PRIV", Priv ? "1" : "0" },
                { "CUSTFLAGS", CustFlags ?? "0" },
                { "SYSFLAGS", GetSysflags() },
                { "EVID", "0" },
                { "EVGID", "0" },
                { "SEED", Seed ?? "0" },
                { "GPSHOST", GPSHost?.Username },
                { "GPSREGION", "0" },
                { "GAMEMODE", "0" },
                { "GAMEPORT", "9673" },
                { "VOIPPORT", "9683" },
                { "PARAMS", Params },
                { "NUMPART", "1" },
                { "PARTSIZE0", MaxSize.ToString() },
                { "PARTPARAMS0", string.Empty },
            };

            foreach (var pair in GetPlayersList())
                OutputCache[pair.Key] = pair.Value;

            return new GenericMessage(msg) { OutputCache = OutputCache };
        }

        public void BroadcastPopulation(MatchmakerServer mc)
        {
            Users.Broadcast(GetGameDetails("+mgm"));
            mc.BroadcastGamesListDetails();
        }

        public void BroadcastPlusSes()
        {
            Users.Broadcast(GetGameDetails("+ses"));
        }

        public static bool IsRanked(string sysFlags)
        {
            return ((string.IsNullOrEmpty(sysFlags) ? 0 : int.Parse(sysFlags)) & (1 << 18)) != 0;
        }

        public bool MatchesSysFlags(string? sysmaskParam, string? sysflagsParam)
        {
            if (string.IsNullOrEmpty(sysmaskParam))
                return true;

            var gameFlags = 0;
            var clientMask = int.Parse(sysmaskParam);

            // Calculate game's flags based on ranked and password status
            if (!string.IsNullOrEmpty(pass))
                gameFlags |= 1 << 16; // Set bit 16 for password
            if (IsRanked(GetSysflags()))
                gameFlags |= 1 << 18; // Set bit 18 for ranked

            // Only check the bits that the client cares about (specified in mask)
            return (gameFlags & clientMask)
                == (
                    (string.IsNullOrEmpty(sysflagsParam) ? 0 : int.Parse(sysflagsParam))
                    & clientMask
                );
        }

        public bool MatchesCustFlags(string? custmaskParam, string? custflagsParam)
        {
            if (string.IsNullOrEmpty(custmaskParam))
                return true;

            var clientMask = int.Parse(custmaskParam);

            // Only check the bits that the client cares about (specified in mask)
            return ((string.IsNullOrEmpty(CustFlags) ? 0 : int.Parse(CustFlags)) & clientMask)
                == (
                    (string.IsNullOrEmpty(custflagsParam) ? 0 : int.Parse(custflagsParam))
                    & clientMask
                );
        }

        public string GetSysflags()
        {
            var sysflags = SysFlags;
            if (Started)
            {
                var gameStartedFlags = 0;
                // Add game started flags (6th, 12th and 19th bits)
                gameStartedFlags |= 1 << 6;
                gameStartedFlags |= 1 << 12;
                gameStartedFlags |= 1 << 19;
                sysflags = gameStartedFlags.ToString();
            }
            if (IsRanked(SysFlags))
                sysflags = (int.Parse(sysflags) | (1 << 18)).ToString(); // Add ranked flag (18th bit)
            if (!string.IsNullOrEmpty(pass))
                sysflags = (int.Parse(sysflags) | (1 << 16)).ToString(); // Add password flag (16th bit)
            return sysflags;
        }

        private Dictionary<string, string> GetPlayersList()
        {
            var i = 0;
            Dictionary<string, string> PLAYERSLIST = new();

            void AddPlayer(int index, AriesUser user, bool isHost)
            {
                PLAYERSLIST.Add($"OPPO{index}", isHost ? '@' + user.Username : user.Username);
                PLAYERSLIST.Add($"OPPART{index}", "0");
                PLAYERSLIST.Add($"OPFLAG{index}", user.Flags);
                PLAYERSLIST.Add($"PRES{index}", "0");
                PLAYERSLIST.Add($"OPID{index}", user.ID.ToString());

                if (Ident.Contains("BURNOUT5"))
                {
                    // Burnout uses a custom function to attribute ther player colors via the server based on player index in the game, thank you Bo98!
                    (bool, string) PlayerColorModifer(int index, string param)
                    {
                        const string playerIndexToChange = "ff";

                        if (index == 3)
                        {
                            if (param.StartsWith(playerIndexToChange))
                                return (
                                    true,
                                    string.Concat($"{user.CurrentGameIndex},", param.AsSpan(2))
                                );
                            return (true, user.CurrentGameIndex.ToString());
                        }
                        else if (index == 2 && param.Length > 1)
                        {
                            if (param.EndsWith(playerIndexToChange))
                                return (
                                    true,
                                    $"{param.Substring(0, param.Length - 2)}{user.CurrentGameIndex},"
                                );
                            return (
                                true,
                                string.Concat(
                                    param.AsSpan(0, param.Length - 1),
                                    user.CurrentGameIndex.ToString()
                                )
                            );
                        }

                        return (false, param);
                    }

                    // On BOP the server is self hosted and accessed via GPSHost params, no need to re-route the Host traffic outside of localhost.
                    PLAYERSLIST.Add($"ADDR{index}", user.ADDR);
                    PLAYERSLIST.Add($"LADDR{index}", user.LADDR);
                    PLAYERSLIST.Add($"MADDR{index}", user.MAC);
                    PLAYERSLIST.Add(
                        $"OPPARAM{index}",
                        user.GetParametersString(PlayerColorModifer)
                    );
                }
                else
                {
                    PLAYERSLIST.Add(
                        $"ADDR{index}",
                        isHost && InternetProtocolUtils.IsLocalhost(user.ADDR) && GPSHost != null // Try to find next available server if possible.
                            ? GPSHost.ADDR
                            : user.ADDR
                    );
                    PLAYERSLIST.Add(
                        $"LADDR{index}",
                        isHost && InternetProtocolUtils.IsLocalhost(user.LADDR) && GPSHost != null // Try to find next available server if possible.
                            ? GPSHost.LADDR
                            : user.LADDR
                    );
                    PLAYERSLIST.Add($"MADDR{index}", user.MAC);
                    PLAYERSLIST.Add($"OPPARAM{index}", user.GetParametersString());
                }
            }

            if (Host != null)
            {
                AddPlayer(i, Host, true);
                i++;
            }

            foreach (var user in Started ? UsersCache : Users.GetAll())
            {
                AddPlayer(i, user, false);
                i++;
            }

            return PLAYERSLIST;
        }
    }
}
