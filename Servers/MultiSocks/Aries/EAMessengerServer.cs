using MultiSocks.Aries.DataStore;
using MultiSocks.Aries.Messages;
using MultiSocks.Aries.Messages.AccountService.ErrorCodes;
using MultiSocks.Aries.Model;
using MultiSocks.Utils;

namespace MultiSocks.Aries
{
    public class EAMessengerServer : AbstractAriesServer
    {
        public override Dictionary<string, Type?> NameToClass { get; } =
            new Dictionary<string, Type?>()
            {
                { "AUTH", typeof(EAMAuth) },
                { "RGET", typeof(Rget) },
                { "PSET", typeof(Pset) },
            };

        public UserCollection Users = new();

        private readonly Thread PingThread;

        public EAMessengerServer(ushort port, string listenIP, string? Project = null, string? SKU = null, bool secure = false, string CN = "", bool WeakChainSignedRSAKey = false) : base(port, listenIP, Project, SKU, secure, CN, WeakChainSignedRSAKey)
        {
            lock (Users)
                Users.AddUser(new AriesUser() { ID = 1 }); // Admin player.

            PingThread = new Thread(PingLoop);
            PingThread.Start();
        }

        public void PingLoop()
        {
            while (true)
            {
                Broadcast(new Ping());
                Thread.Sleep(30000);
            }
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
            }
        }

        public void TryEAMLogin(DbAccount user, AriesClient client, string? PASS, string LOC, string? MAC, string? TOKEN)
        {
            //is someone else already logged in as this user?
            AriesUser? oldUser = Users.GetUserByName(user.Username);
            if (oldUser != null)
            {
                client.SendMessage(new AuthLogn());
                return;
            }

            string? DecryptedPass = PasswordUtils.Ssc2Decode(PASS, client.SKEY);

            if (DecryptedPass == string.Empty) // EA assumed that Consoles protect the login so they crypt an empty password, extremly bad, but can't do anything.
            {

            }
            else if (user.Password != DecryptedPass)
            {
                client.SendMessage(new AuthPass());
                return;
            }

            CustomLogger.LoggerAccessor.LogInfo("EA Messenger Logged in: " + user.Username);

            string[] personas = new string[4];
            for (int i = 0; i < user.Personas.Count; i++)
            {
                personas[i] = user.Personas[i];
            }

            //make a user object from DB user
            AriesUser user2 = new()
            {
                MAC = MAC ?? string.Empty,
                Connection = client,
                ID = user.ID,
                Personas = personas,
                Friends = user.Friends,
                Rivals = user.Rivals,
                Username = user.Username,
                ADDR = client.ADDR,
                LADDR = client.LADDR,
                LOC = LOC
            };

            Users.AddUser(user2);
            client.User = user2;

            // Ideally all the infos comes from User profile and not hardcodded.

            client.SendMessage(new EAMAuth()
            {
                BORN = "19800325",
                GEND = "M",
                FROM = "US",
                LANG = "en",
                LAST = DateTime.Now.ToString("yyyy.M.d HH:mm:ss"),
                TOS = user.TOS,
                NAME = user.Username,
                MAIL = user.MAIL,
                PERSONAS = string.Join(',', user.Personas)
            });
        }
    }
}
