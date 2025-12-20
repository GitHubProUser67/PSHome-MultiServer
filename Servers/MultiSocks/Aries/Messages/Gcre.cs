using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Gcre : AbstractMessage
    {
        public override string _Name { get => "gcre"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            string? USERPARAMS = GetInputCacheValue("USERPARAMS");
            string? FORCE_LEAVE = GetInputCacheValue("FORCE_LEAVE");
            string? SYSFLAGS = GetInputCacheValue("SYSFLAGS");
            string? PASS = GetInputCacheValue("PASS");
            string? PARAMS = GetInputCacheValue("PARAMS");
            string? NAME = GetInputCacheValue("NAME");

            if (!string.IsNullOrEmpty(USERPARAMS))
                user.SetParametersFromString(USERPARAMS);

            if (!string.IsNullOrEmpty(FORCE_LEAVE) && FORCE_LEAVE == "1" && user.CurrentGame != null)
            {
                AriesGame prevGame = user.CurrentGame;

                if (prevGame.RemovePlayerByUsername(user.Username))
                    mc.Games.RemoveGame(prevGame);
            }

            int? parsedPriv = int.TryParse(GetInputCacheValue("PRIV"), out int priv) ? priv : 0;

            if (int.TryParse(GetInputCacheValue("MINSIZE"), out int minSize) && int.TryParse(GetInputCacheValue("MAXSIZE"), out int maxSize)
                && !string.IsNullOrEmpty(PARAMS) && !string.IsNullOrEmpty(NAME) && !string.IsNullOrEmpty(SYSFLAGS))
            {
                AriesGame? game = mc.Games.AddGame(maxSize, minSize, GetInputCacheValue("CUSTFLAGS"), PARAMS, NAME, parsedPriv.Value != 0, GetInputCacheValue("SEED"), SYSFLAGS, PASS, user.CurrentRoom?.ID ?? 0);

                if (game != null)
                {
                    if (game.MinSize > 1 && game.Users.GetUserByName("brobot24") == null)
                        game.AddHost(mc.Users.GetUserByName("brobot24"));

                    if (game.Users.GetUserByName(user.Username) == null)
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
