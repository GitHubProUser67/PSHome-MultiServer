using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Components
{
    public class Gjoi : AbstractMessage
    {
        public override string _Name
        {
            get => "gjoi";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc)
                return;

            var user = client.User;
            if (user == null)
                return;

            var FORCE_LEAVE = GetInputCacheValue("FORCE_LEAVE");
            var SESS = GetInputCacheValue("SESS");
            var SEED = GetInputCacheValue("SEED");
            var PASS = GetInputCacheValue("PASS");
            var NAME = GetInputCacheValue("NAME");
            var PARAMS = GetInputCacheValue("PARAMS");
            var SYSFLAGS = GetInputCacheValue("SYSFLAGS");

            if (
                !string.IsNullOrEmpty(FORCE_LEAVE)
                && FORCE_LEAVE == "1"
                && user.CurrentGame != null
            )
            {
                var prevGame = user.CurrentGame;

                if (prevGame.RemovePlayerByUsername(user.Username))
                    mc.Games.RemoveGame(prevGame);
            }

            if (
                int.TryParse(GetInputCacheValue("MINSIZE"), out var minSize)
                && int.TryParse(GetInputCacheValue("MAXSIZE"), out var maxSize)
                && int.TryParse(GetInputCacheValue("PRIV"), out var priv)
                && int.TryParse(GetInputCacheValue("IDENT"), out var ident)
            )
            {
                if ("Invite".Equals(SESS))
                {
                    AriesGame? game = mc
                        .Games.GamesSessions.Values.Where(game =>
                            game.pass == PASS
                            && game.ID == ident
                            && game.Priv == (priv == 1)
                            && game.Seed == SEED
                        )
                        .FirstOrDefault();

                    if (game != null)
                    {
                        if (game.GetActiveUsersCount() >= game.MaxSize)
                            client.SendMessage(new GjoiFull());
                        else
                        {
                            game.AddUser(user);

                            user.CurrentGame = game;

                            client.SendMessage(game.GetGameDetails("gjoi"));

                            user.SendPlusWho(user, context.Project);

                            game.BroadcastPopulation(mc);
                        }

                        return;
                    }
                }
                else if (
                    int.TryParse(GetInputCacheValue("ROOM"), out var room)
                    && !string.IsNullOrEmpty(PARAMS)
                    && !string.IsNullOrEmpty(NAME)
                    && !string.IsNullOrEmpty(SYSFLAGS)
                )
                {
                    AriesGame? game = mc
                        .Games.GamesSessions.Values.Where(game =>
                            game.Name == NAME
                            && game.pass == PASS
                            && game.CustFlags == GetInputCacheValue("CUSTFLAGS")
                            && game.GetSysflags() == SYSFLAGS
                            && game.Params == PARAMS
                            && game.RoomID == room
                            && game.ID == ident
                            && game.Priv == (priv == 1)
                            && game.Seed == SEED
                        )
                        .FirstOrDefault();

                    if (game != null)
                    {
                        if (game.GetActiveUsersCount() >= game.MaxSize)
                            client.SendMessage(new GjoiFull());
                        else
                        {
                            game.AddUser(user);

                            user.CurrentGame = game;

                            client.SendMessage(game.GetGameDetails(_Name));

                            user.SendPlusWho(user, context.Project);

                            game.BroadcastPopulation(mc);
                        }

                        return;
                    }
                }
            }

            client.SendMessage(new GjoiUgam());
        }
    }
}
