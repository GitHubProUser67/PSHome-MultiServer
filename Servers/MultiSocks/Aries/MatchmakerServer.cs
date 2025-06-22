using MultiSocks.Aries.DataStore;
using MultiSocks.Aries.Messages;
using MultiSocks.Aries.Model;
using MultiSocks.Utils;
using NetHasher.CRC;
using System.Text;

namespace MultiSocks.Aries
{
    public class MatchmakerServer : AbstractAriesServer
    {
        private readonly UniqueIDGenerator _guestIdGen = new UniqueIDGenerator();

        public override Dictionary<string, Type?> NameToClass { get; } =
            new Dictionary<string, Type?>()
            {
                { "~png", typeof(Ping) },
                { "move", typeof(Move) }, //move into a room. remove last room and broadcast "+rom" to others. broadcast "+pop" with Z=ID/Count for population update
                { "mesg", typeof(Mesg) }, //PRIV is non-null for private. else broadcast to room. PRIV->(find client), TEXT->T, ATTR->EP, (name)->N
                { "room", typeof(Room) }, //create room. NAME=name, return (room, +who, +msg, +rom to all, +usr)
                { "auxi", typeof(Auxi) }, //auxiliary information. returned as X attribute in +usr and +who
                { "auth", typeof(Auth) },
                { "acct", typeof(Acct) },
                { "cate", typeof(Cate) }, //?
                { "conn", null }, //?
                { "cusr", null }, //?
                { "cper", typeof(Cper) }, //create persona. (NAME) in, (PERS, NAME) out. where name is username.
                { "dper", typeof(Dper) }, //delete persona
                { "edit", null }, //?
                { "fbst", typeof(Fbst) }, //?
                { "fget", typeof(Fget) }, // get friend/rival list
                { "fupd", typeof(Fupd) }, // Update friend/rival lists in user record
                { "fupr", typeof(Fupr) }, //?
                { "hchk", typeof(Hchk) }, //?
                { "peek", typeof(Peek) }, //Audit room and receive infos about it, but not enter.
                { "pent", typeof(Pent) }, // Purchase entitlement.
                { "pers", typeof(Pers) }, //select persona
                { "sdta", typeof(Sdta) }, //?
                { "sele", typeof(SeleIn) }, //gets info for the current server
                { "skey", typeof(SKey) }, //session key?
                { "slst", typeof(Slst) }, //lobby details?
                { "sviw", typeof(Sviw) }, // Request names for each fields in a user's stats record.
                { "user", typeof(User) }, //get my user info
                { "usld", typeof(Usld) }, // User Settings Load -- load a user's (persona's) common game specific settings.
                { "onln", typeof(Onln) }, //search for a user's info
                { "opup", typeof(Opup) }, //?
                { "ottr", typeof(Ottr) }, //?
                { "rent", typeof(Rent) }, // Refresh Entitlements
                { "rrlc", typeof(Rrlc) }, // (CUSTOM Burnout Paradise) Road Rules Local
                { "rrup", typeof(Rrup) }, // (CUSTOM Burnout Paradise) Road Rules Upload
                { "rrgt", typeof(Rrgt) }, // (CUSTOM Burnout Paradise) Road Rules Get
                { "rvup", typeof(Rvup) }, // (CUSTOM Burnout Paradise) RiVal UPload
                { "addr", typeof(Addr) }, //the client tells us their IP and port (ephemeral). The IP is usually wrong.
                { "chal", typeof(ChalIn) }, //enter challenge mode
                { "glea", typeof(Glea) }, //leave game (string NAME)
                { "gget", null }, //get game by name
                { "gjoi", typeof(Gjoi) }, //join game by name
                { "gdel", typeof(Gdel) }, //delete game by name
                { "gsea", typeof(Gsea) }, //Game search
                { "gset", typeof(Gset) }, //Actualize Game properties.
                { "gsta", typeof(Gsta) }, //game start. return gstanepl if not enough people, empty gsta if enough.
                { "gcre", typeof(Gcre) }, //game create. (name, roomname, maxPlayers, minPlayers, sysFlags, params). return a lot of info
                { "gpsc", typeof(Gpsc) }, // Create a game on a persistent game spawn service for a user.
                { "gqwk", typeof(Gqwk) }, // Join the best matching game based on provided criteria.
                { "news", typeof(NewsIn) }, //news for server. return newsnew0 with news info (plaintext mode, NOT keyvalue)
                { "rank", typeof(Rank) }, //unknown. { RANK = "Unranked", TIME = 866 }
                { "tcup", typeof(Tcup) }, // Time Challenge Score Upload?
                { "snap", typeof(Snap) }, // Get Leaderboard snapshot
                { "quik", typeof(Quik) }, // Old version Quick Match
                { "rept", typeof(Rept) }, // Submit a Report about a user
                { "rcat", typeof(Rcat) }, // Fetch room category information
                { "priv", typeof(Priv) }, // Set Private Message mode.
                { "qdef", typeof(Qdef) }, // Quick Message Defaults -- load a user's default quickmessages.
                { "flag", typeof(Flag) }, // Set attribute flags.
                { "ucre", typeof(Ucre) }, // Create a new user set. A user set maps a set of online users into a group.
                { "uatr", typeof(Uatr) }, // Update user attributes and hardware flags
                { "lggr", typeof(Lggr) } // Client -> Server logger 
            };

        public UserCollection Users = new();
        public RoomCollection Rooms = new();
        public GameCollection Games = new();

        private readonly Thread PingThread;

        public MatchmakerServer(ushort port, string listenIP, List<Tuple<string, bool>>? RoomToAdd = null, string? Project = null, string? SKU = null, bool secure = false, string CN = "", bool WeakChainSignedRSAKey = false) : base(port, listenIP, Project, SKU, secure, CN, WeakChainSignedRSAKey)
        {
            Rooms.Server = this;

            lock (Users)
                Users.AddUser(new AriesUser() { ID = 1 }); // Admin player.

            PingThread = new Thread(PingLoop);
            PingThread.Start();

            lock (Rooms)
            {
                if (RoomToAdd != null && RoomToAdd.Count > 0)
                {
                    foreach (var pair in RoomToAdd)
                    {
                        CustomLogger.LoggerAccessor.LogInfo($"[MatchmakerServer] - Adding Room: {pair.Item1}, {(pair.Item2 ? ("With Global Availability: " + pair.Item2 + " ") : string.Empty)}on Port: {port}");
                        Rooms.AddRoom(new AriesRoom() { Name = pair.Item1, IsGlobal = pair.Item2 });
                    }
                }
                else
                    Rooms.AddRoom(new AriesRoom() { Name = "room", IsGlobal = true });
            }
        }

        public void PingLoop()
        {
            while (true)
            {
                Broadcast(new Ping());
                Thread.Sleep(30000);
            }
        }

        public Task BroadcastGamesListDetails()
        {
            foreach (AriesGame game in Games.GamesSessions.Values)
            {
                Broadcast(game.GetGameDetails("+gam"));
            }

            return Task.CompletedTask;
        }

        public override void RemoveClient(AriesClient client)
        {
            base.RemoveClient(client);

            //clean up this user's state.
            //are they logged in?
            AriesUser? user = client.User;
            if (user != null)
            {
                Users.RemoveUser(user);

                AriesGame? game = user.CurrentGame;
                AriesRoom? room = user.CurrentRoom;
                if (game != null && game.RemoveUserAndCheckGameValidity(user))
                    Games.RemoveGame(game);

                if (room != null && room.Users.RemoveUserAndCheckRoomValidity(user))
                   Rooms.RemoveRoom(room);
            }
        }

        public void SendToPersona(string name, AbstractMessage msg)
        {
            AriesUser? user = Users.GetUserByPersonaName(name);
            user?.Connection?.SendMessage(msg);
        }

        public void TryLogin(DbAccount user, AriesClient client, string? PASS, string LOC, string? MAC, string? TOKEN)
        {
            //is someone else already logged in as this user?
            AriesUser? oldUser = Users.GetUserByName(user.Username);
            if (oldUser != null)
            {
                client.SendMessage(new AuthLogn());
                return;
            }

            string? DecryptedPass = new PasswordUtils().ssc2Decode(PASS, client.SKEY);

            if (DecryptedPass == string.Empty) // EA assumed that Consoles protect the login so they crypt an empty password, extremly bad, but can't do anything.
            {

            }
            else if (user.Password != DecryptedPass)
            {
                client.SendMessage(new AuthPass());
                return;
            }

            CustomLogger.LoggerAccessor.LogInfo("Logged in: " + user.Username);

            string[] personas = new string[user.Personas.Count];
            for (int i = 0; i < user.Personas.Count; i++)
            {
                personas[i] = user.Personas[i];
            }

            // make a user object from DB user
            AriesUser user2 = new()
            {
                MAC = MAC ?? string.Empty,
                Connection = client,
                ID = user.ID,
                Personas = personas,
                Username = user.Username,
                ADDR = client.ADDR,
                LADDR = client.LADDR,
                LOC = LOC
            };

            Users.AddUser(user2);
            client.User = user2;

            Dictionary<string, string?> OutputCache;

            if (client.VERS.Contains("BURNOUT5") || client.VERS.Contains("DPR-09") || (client.VERS.Contains("NASCAR09") && client.SKU == "PS3"))
            {
                OutputCache = new() {
                { "LAST", DateTime.Now.ToString("yyyy.M.d-HH:mm:ss") },
                { "TOS", user.TOS },
                { "SHARE", user.SHARE },
                { "_LUID", "$00000000000003fe" },
                { "NAME", user.Username },
                { "PERSONAS", string.Join(',', user.Personas) },
                { "MAIL", user.MAIL }, 
                { "BORN", "19700101" },
                { "FROM", user2.LOC[2..] },
                { "LOC", user2.LOC },
                { "SPAM", "NN" },
                { "SINCE", "2008.1.1-00:00:00" },
                { "GFIDS", "1" },
                { "ADDR", client.ADDR },
                { "TOKEN", (!string.IsNullOrEmpty(TOKEN)) ? TOKEN : "pc6r0gHSgZXe1dgwo_CegjBCn24uzUC7KVq1LJDKJ0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" }};

                client.SendMessage(new Auth() { OutputCache = OutputCache });
            }
            else
            {
                OutputCache = new() {
                { "BORN", "19800325" },
                { "GEND", "M" },
                { "FROM", user2.LOC[2..] },
                { "LANG", user2.LOC[..2] },
                { "LAST", DateTime.Now.ToString("yyyy.M.d HH:mm:ss") },
                { "TOS", user.TOS },
                { "NAME", user.Username },
                { "MAIL", user.MAIL },
                { "PERSONAS", string.Join(',', user.Personas) }};

                client.SendMessage(new Auth() { OutputCache = OutputCache });

                Rooms.SendRooms(user2);
            }
        }

        public void TryGuestLogin(AriesClient client, string? PASS, string LOC, string? MAC, string TOKEN)
        {
            // From: https://github.com/teknogods/eaEmu/blob/master/eaEmu/ea/games/pcburnout08.py#L184
            string guestUsername = $"guest{_guestIdGen.CreateUniqueID()}";

            //is someone else already logged in as this user?
            AriesUser? oldUser = Users.GetUserByName(guestUsername);
            if (oldUser != null)
            {
                client.SendMessage(new AuthLogn());
                return;
            }

            CustomLogger.LoggerAccessor.LogInfo("Logged in: " + guestUsername);

            string[] personas = new string[1] { guestUsername };

            // make a user object from DB user
            AriesUser user2 = new()
            {
                MAC = MAC ?? string.Empty,
                Connection = client,
                ID = (int)CRC32.CreateCastagnoli(Encoding.UTF8.GetBytes(guestUsername + (new PasswordUtils().ssc2Decode(PASS, client.SKEY) ?? string.Empty) + "EA")),
                Personas = personas,
                Username = guestUsername,
                ADDR = client.ADDR,
                LADDR = client.LADDR,
                LOC = LOC
            };

            Users.AddUser(user2);
            client.User = user2;

            Dictionary<string, string?> OutputCache;

            if (client.VERS.Contains("BURNOUT5"))
            {
                OutputCache = new() {
                { "LAST", DateTime.Now.ToString("yyyy.M.d-HH:mm:ss") },
                { "TOS", "1" },
                { "SHARE", "1" },
                { "_LUID", "$00000000000003fe" },
                { "NAME", guestUsername },
                { "PERSONAS", string.Join(',', personas) },
                { "MAIL", "user@domain.com" },
                { "BORN", "19700101" },
                { "FROM", user2.LOC[2..] },
                { "LOC", user2.LOC },
                { "SPAM", "NN" },
                { "SINCE", "2008.1.1-00:00:00" },
                { "GFIDS", "1" },
                { "ADDR", client.ADDR },
                { "TOKEN", TOKEN }};

                client.SendMessage(new Auth() { OutputCache = OutputCache });
            }
            else
            {
                OutputCache = new() {
                { "BORN", "19800325" },
                { "GEND", "M" },
                { "FROM", user2.LOC[2..] },
                { "LANG", user2.LOC[..2] },
                { "LAST", DateTime.Now.ToString("yyyy.M.d HH:mm:ss") },
                { "TOS", "1" },
                { "NAME", guestUsername },
                { "MAIL", "user@domain.com" },
                { "PERSONAS", string.Join(',', personas) }};

                client.SendMessage(new Auth() { OutputCache = OutputCache });

                Rooms.SendRooms(user2);
            }
        }
    }
}
