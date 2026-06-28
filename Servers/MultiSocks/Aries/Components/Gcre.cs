namespace MultiSocks.Aries.Components
{
    public class Gcre : AbstractMessage
    {
        public override string _Name
        {
            get => "gcre";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc)
                return;

            var user = client.User;
            if (user == null)
                return;

            var USERPARAMS = GetInputCacheValue("USERPARAMS");
            var FORCE_LEAVE = GetInputCacheValue("FORCE_LEAVE");
            var SYSFLAGS = GetInputCacheValue("SYSFLAGS");
            var PASS = GetInputCacheValue("PASS");
            var PARAMS = GetInputCacheValue("PARAMS");
            var NAME = GetInputCacheValue("NAME");

            if (!string.IsNullOrEmpty(USERPARAMS))
                user.SetParametersFromString(USERPARAMS);

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

            int? parsedPriv = int.TryParse(GetInputCacheValue("PRIV"), out var priv) ? priv : 0;

            if (
                int.TryParse(GetInputCacheValue("MINSIZE"), out var minSize)
                && int.TryParse(GetInputCacheValue("MAXSIZE"), out var maxSize)
                && !string.IsNullOrEmpty(PARAMS)
                && !string.IsNullOrEmpty(NAME)
                && !string.IsNullOrEmpty(SYSFLAGS)
            )
            {
                var game = mc.Games.AddGame(
                    maxSize,
                    minSize,
                    user.Connection?.Context.Project ?? string.Empty,
                    user.Connection?.Context.SKU,
                    GetInputCacheValue("CUSTFLAGS"),
                    PARAMS,
                    NAME,
                    parsedPriv.Value != 0,
                    GetInputCacheValue("SEED"),
                    SYSFLAGS,
                    PASS,
                    user.CurrentRoom?.ID ?? 0
                );

                if (game != null)
                {
                    if (game.MinSize > 1)
                    {
                        /*
                         * Not working properly (different host is needed)
                         * if (!string.IsNullOrEmpty(user.Connection?.Context.Project) &&
                            user.Connection.Context.Project.Contains("BURNOUT5"))
                            game.AddHost(mc.Users.GetUserByName("brobot24"));
                        else
                            game.AddHost(user);*/

                        game.AddHost(mc.Users.GetUserByName("brobot24"));
                    }

                    game.AddGPSHost(user);

                    user.CurrentGame = game;

                    client.SendMessage(game.GetGameDetails(_Name));

                    user.SendPlusWho(user, context.Project);

                    game.BroadcastPopulation(mc);
                }
                else
                    client.SendMessage(new GcreDupl());
            }
            else
                client.SendMessage(new GcreInvp());
        }
    }
}
