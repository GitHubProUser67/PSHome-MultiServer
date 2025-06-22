using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Gset : AbstractMessage
    {
        public override string _Name { get => "gset"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            string? PERS = GetInputCacheValue("PERS");
            string? USERPARAMS = GetInputCacheValue("USERPARAMS");
            string? USERFLAGS = GetInputCacheValue("USERFLAGS");
            string? NAME = GetInputCacheValue("NAME");
            string? PARAMS = GetInputCacheValue("PARAMS");
            string? PASS = GetInputCacheValue("PASS");
            string? FORCE_LEAVE = GetInputCacheValue("FORCE_LEAVE");
            string? KICK = GetInputCacheValue("KICK");
            string? SYSFLAGS = GetInputCacheValue("SYSFLAGS");

            AriesUser? user = string.IsNullOrEmpty(PERS) ? client.User : mc.Users.GetUserByPersonaName(PERS);
            if (user == null) return;

            if (!string.IsNullOrEmpty(USERPARAMS))
                user.SetParametersFromString(USERPARAMS);
            if (!string.IsNullOrEmpty(USERFLAGS))
                user.Flags = USERFLAGS;

            if (!string.IsNullOrEmpty(KICK) && user.CurrentGame != null)
            {
                foreach (string player in KICK.Split(','))
                {
                    if (user.CurrentGame!.RemovePlayerByUsername(player, 1, GetInputCacheValue("KICK_REASON")))
                        mc.Games.RemoveGame(user.CurrentGame);
                }
            }

            if (user.CurrentGame != null)
            {
                if (!string.IsNullOrEmpty(SYSFLAGS))
                    user.CurrentGame.SysFlags = SYSFLAGS;

                if (int.TryParse(GetInputCacheValue("MINSIZE"), out int minSize) && int.TryParse(GetInputCacheValue("MAXSIZE"), out int maxSize)
                    && int.TryParse(GetInputCacheValue("ROOM"), out int room) && int.TryParse(GetInputCacheValue("IDENT"), out int ident)
                    && int.TryParse(GetInputCacheValue("PRIV"), out int priv) && !string.IsNullOrEmpty(PARAMS) && !string.IsNullOrEmpty(NAME))
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
                    AriesGame prevGame = user.CurrentGame;

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
