namespace MultiSocks.Aries.Components
{
    public class Gset : AbstractMessage
    {
        public override string _Name
        {
            get => "gset";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc)
                return;

            var PERS = GetInputCacheValue("PERS");
            var USERPARAMS = GetInputCacheValue("USERPARAMS");
            var USERFLAGS = GetInputCacheValue("USERFLAGS");
            var NAME = GetInputCacheValue("NAME");
            var PARAMS = GetInputCacheValue("PARAMS");
            var PASS = GetInputCacheValue("PASS");
            var FORCE_LEAVE = GetInputCacheValue("FORCE_LEAVE");
            var KICK = GetInputCacheValue("KICK");
            var SYSFLAGS = GetInputCacheValue("SYSFLAGS");

            var user = string.IsNullOrEmpty(PERS)
                ? client.User
                : mc.Users.GetUserByPersonaName(PERS);
            if (user == null)
                return;

            if (!string.IsNullOrEmpty(USERPARAMS))
                user.SetParametersFromString(USERPARAMS);
            if (!string.IsNullOrEmpty(USERFLAGS))
                user.Flags = USERFLAGS;

            if (!string.IsNullOrEmpty(KICK) && user.CurrentGame != null)
            {
                foreach (var player in KICK.Split(','))
                {
                    if (
                        user.CurrentGame!.RemovePlayerByUsername(
                            player,
                            1,
                            GetInputCacheValue("KICK_REASON")
                        )
                    )
                        mc.Games.RemoveGame(user.CurrentGame);
                }
            }

            if (user.CurrentGame != null)
            {
                if (!string.IsNullOrEmpty(SYSFLAGS))
                    user.CurrentGame.SysFlags = SYSFLAGS;

                if (
                    int.TryParse(GetInputCacheValue("MINSIZE"), out var minSize)
                    && int.TryParse(GetInputCacheValue("MAXSIZE"), out var maxSize)
                    && int.TryParse(GetInputCacheValue("ROOM"), out var room)
                    && int.TryParse(GetInputCacheValue("IDENT"), out var ident)
                    && int.TryParse(GetInputCacheValue("PRIV"), out var priv)
                    && !string.IsNullOrEmpty(PARAMS)
                    && !string.IsNullOrEmpty(NAME)
                )
                {
                    mc.Games.TryChangeGameId(user.CurrentGame, ident);
                    user.CurrentGame.Name = NAME;
                    user.CurrentGame.pass = PASS;
                    user.CurrentGame.MinSize = minSize;
                    user.CurrentGame.MaxSize = maxSize;
                    user.CurrentGame.CustFlags = GetInputCacheValue("CUSTFLAGS");
                    user.CurrentGame.RoomID = room;
                    user.CurrentGame.Priv = priv == 1;
                    user.CurrentGame.Seed = GetInputCacheValue("SEED");
                    user.CurrentGame.Params = PARAMS;
                }
                // Force leave is also sent for classic update packets, so we apply it only when it makes sense.
                else if (!string.IsNullOrEmpty(FORCE_LEAVE) && FORCE_LEAVE == "1")
                {
                    var prevGame = user.CurrentGame;

                    if (prevGame.RemovePlayerByUsername(user.Username))
                        mc.Games.RemoveGame(prevGame);
                }

                if (user.CurrentGame != null)
                {
                    client.SendMessage(user.CurrentGame.GetGameDetails(_Name));

                    user.CurrentGame.BroadcastPopulation(mc);
                }
            }
        }
    }
}
