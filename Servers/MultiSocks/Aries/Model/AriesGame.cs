using MultiSocks.Aries.Messages;
using System.Collections.Concurrent;

namespace MultiSocks.Aries.Model
{
    public class AriesGame
    {
        public int MaxSize;
        public int MinSize;
        public int ID;
        public int RoomID;
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
        private ConcurrentDictionary<int, bool> _pIdIsUsed = new();

        private readonly object _ClientIndexlock = new();

        public AriesGame(int maxSize, int minSize, int id, string? custFlags, string @params,
                string name, bool priv, string? seed, string sysFlags, string? Pass, int roomId)
        {
            MaxSize = maxSize;
            MinSize = minSize;
            ID = id;
            CustFlags = custFlags;
            Params = @params;
            Name = name;
            Priv = priv;
            Seed = seed;
            SysFlags = sysFlags;
            pass = Pass;
            RoomID = roomId;

            // populate collection of used player ids
            for (int i = 0; i < maxSize; ++i)
                _pIdIsUsed.TryAdd(i, false);
        }

        private bool TryRegisterNewClientIndex(out int index)
        {
            lock (_ClientIndexlock)
            {
                for (index = 0; index < _pIdIsUsed.Count; ++index)
                {
                    if (_pIdIsUsed.TryGetValue(index, out bool isUsed) && !isUsed)
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
        public bool RemoveUserAndCheckGameValidity(AriesUser user, int reason = 0, string? KickReason = "")
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

                if (Users.Count() < MinSize || GPSHost == user) // End Game.
                {
                    user.SendPlusWho(user, user.Connection?.Context.Project);

                    foreach (AriesUser batchuser in Users.GetAll())
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

            Users.AddUser(user);
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

        public bool RemovePlayerByUsername(string? username, int reason = 0, string? KickReason = "")
        {
            if (string.IsNullOrEmpty(username)) return false;

            AriesUser? userToRemove = Users.GetUserByName(username);

            if (userToRemove != null)
                return RemoveUserAndCheckGameValidity(userToRemove, reason, KickReason);

            return false;
        }

        public void SetGameStatus(bool status)
        {
            Started = status;

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
                { "COUNT", Users?.Count().ToString() ?? "1" },
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
                { "PARTPARAMS0", string.Empty }
            };

            foreach (var pair in GetPlayersList())
            {
                OutputCache[pair.Key] = pair.Value;
            }

            return new GenericMessage(msg)
            {
                OutputCache = OutputCache
            };
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

        public bool IsRanked(string sysFlags)
        {
            return ((string.IsNullOrEmpty(sysFlags) ? 0 : int.Parse(sysFlags)) & (1 << 18)) != 0;
        }

        public bool MatchesSysFlags(string? sysmaskParam, string? sysflagsParam)
        {
            if (string.IsNullOrEmpty(sysmaskParam))
                return true;

            int gameFlags = 0;
            int clientMask = int.Parse(sysmaskParam);

            // Calculate game's flags based on ranked and password status
            if (!string.IsNullOrEmpty(pass))
                gameFlags |= (1 << 16);  // Set bit 16 for password
            if (IsRanked(GetSysflags()))
                gameFlags |= (1 << 18);  // Set bit 18 for ranked

            // Only check the bits that the client cares about (specified in mask)
            return (gameFlags & clientMask) == ((string.IsNullOrEmpty(sysflagsParam) ? 0 : int.Parse(sysflagsParam)) & clientMask);
        }

        public bool MatchesCustFlags(string? custmaskParam, string? custflagsParam)
        {
            if (string.IsNullOrEmpty(custmaskParam))
                return true;

            int clientMask = int.Parse(custmaskParam);

            // Only check the bits that the client cares about (specified in mask)
            return ((string.IsNullOrEmpty(CustFlags) ? 0 : int.Parse(CustFlags)) & clientMask) == ((string.IsNullOrEmpty(custflagsParam) ? 0 : int.Parse(custflagsParam)) & clientMask);
        }

        public string GetSysflags()
        {
            string sysflags = SysFlags;
            if (Started)
            {
                int gameStartedFlags = 0;
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
            int i = 0;
            Dictionary<string, string> PLAYERSLIST = new();

            foreach (AriesUser user in Started ? UsersCache : Users.GetAll())
            {
                PLAYERSLIST.Add($"OPPO{i}", user == Host ? '@' + user.Username : user.Username);
                PLAYERSLIST.Add($"OPPART{i}", "0");
                PLAYERSLIST.Add($"OPFLAG{i}", user.Flags);
                PLAYERSLIST.Add($"PRES{i}", "0");
                PLAYERSLIST.Add($"OPID{i}", user.ID.ToString());
                PLAYERSLIST.Add($"ADDR{i}", user.ADDR);
                PLAYERSLIST.Add($"LADDR{i}", user.LADDR);
                PLAYERSLIST.Add($"MADDR{i}", user.MAC);

                if (!string.IsNullOrEmpty(user.Connection?.Context.Project) && user.Connection.Context.Project.Contains("BURNOUT5"))
                {
                    // Burnout uses a custom function to attribute ther player colors via the server based on player index in the game, thank you Bo98!
                    (bool, string) PlayerColorModifer(int index, string param)
                    {
                        const string playerIndexToChange = "ff";

                        if (index == 3)
                        {
                            if (param.StartsWith(playerIndexToChange))
                                return (true, string.Concat($"{user.CurrentGameIndex},", param.AsSpan(2)));
                            return (true, user.CurrentGameIndex.ToString());
                        }
                        else if (index == 2 && param.Length > 1)
                        {
                            if (param.EndsWith(playerIndexToChange))
                                return (true, $"{param.Substring(0, param.Length - 2)}{user.CurrentGameIndex},");
                            return (true, param.Substring(0, param.Length - 1) + user.CurrentGameIndex.ToString());
                        }

                        return (false, param);
                    }

                    PLAYERSLIST.Add($"OPPARAM{i}", user.GetParametersString(PlayerColorModifer));
                }
                else
                    PLAYERSLIST.Add($"OPPARAM{i}", user.GetParametersString());

                i++;
            }

            return PLAYERSLIST;
        }
    }
}
