using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Components
{
    public class ChalIn : AbstractMessage
    {
        public override string _Name
        {
            get => "chal";
        }

        public string? _From;
        public AriesUser? _FromUser;

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            var HOST = GetInputCacheValue("HOST");
            var KIND = GetInputCacheValue("KIND");
            var PERS = GetInputCacheValue("PERS");

            if (context is not MatchmakerServer mc)
            {
                client.SendMessage(new ChalOut() { MODE = "idle" });
                return;
            }

            var user = client.User;
            if (user == null)
            {
                client.SendMessage(new ChalOut() { MODE = "idle" });
                return;
            }

            var room = user.CurrentRoom;
            if (room == null)
            {
                client.SendMessage(new ChalOut() { MODE = "idle" });
                return;
            }

            _From = user.PersonaName;
            _FromUser = user;

            lock (room.ChallengeMap)
            {
                room.RemoveChallenges(user); //remove any challenges we made before
                if (PERS == "*")
                {
                    //we don't want to play with anyone anymore
                    client.SendMessage(new ChalOut() { MODE = "0" });

                    return;
                }
                //firstly, is someone wanting to play with us yet
                else if (
                    user.PersonaName != null
                    && room.ChallengeMap.TryGetValue(user.PersonaName, out var other)
                )
                {
                    if (PERS == other._From)
                    {
                        //start the session.
                        var chals = new ChalIn[] { this, other };
                        var host = chals.FirstOrDefault(x => x.GetInputCacheValue("HOST") == "1");
                        var users = chals.Select(x => x._FromUser);
                        if (host == null)
                            return; //??

                        if (room.AllInGame)
                        {
                            users = room.Users?.GetAll();
                            CustomLogger.LoggerAccessor.LogInfo(
                                "Starting an all play game session: "
                                    + string.Join(',', users.Select(x => x.PersonaName))
                            );
                        }
                        else
                            CustomLogger.LoggerAccessor.LogInfo(
                                "Starting a private game session between " + _From + " and " + PERS
                            );

                        PlusSes sess = new()
                        {
                            IDENT = "1",
                            HOST = host._From,
                            ROOM = room.Name,
                            KIND = host.GetInputCacheValue("KIND"),
                            COUNT = users.Count().ToString(),

                            OPID = users.Select(x => x?.ID.ToString()).ToArray(),
                            OPPO = users.Select(x => x?.PersonaName).ToArray(),
                            ADDR = users.Select(x => x?.Connection?.ADDR).ToArray(),

                            SEED = new Random().Next().ToString(),
                            SELF = user.PersonaName,
                        };

                        /*
                         * Experimental stuff to try get bustin out to connect multiple users
                         * didn't work unfortunately.
                        if (room.AllInGame)
                        {
                            //send a packet containing everyone to the host
                            sess.SELF = host._From;
                            var hostuser = host._FromUser;
                            hostuser.Connection?.SendMessage(sess);

                            //send a packet containing only the host and self to everyone else
                            foreach (var userx in users)
                            {
                                if (userx == hostuser) continue;
                                sess.SELF = userx.PersonaName;

                                var nusers = new List<User>() { hostuser, userx };

                                sess.COUNT = "2";
                                sess.OPID = nusers.Select(x => x.ID.ToString()).ToArray();
                                sess.OPPO = nusers.Select(x => x.PersonaName).ToArray();
                                sess.ADDR = nusers.Select(x => x.IP).ToArray();

                                userx.Connection?.SendMessage(sess);
                            }
                        }
                        else
                        {*/
                        foreach (var userx in users)
                        {
                            sess.SELF = userx.PersonaName;
                            userx.Connection?.SendMessage(sess);
                        }
                        //}

                        return;
                    }
                }

                //otherwise let's add this
                room.ChallengeMap[PERS] = this;
            }
        }
    }
}
