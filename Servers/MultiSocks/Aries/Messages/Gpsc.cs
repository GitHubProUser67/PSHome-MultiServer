using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Gpsc : AbstractMessage
    {
        public override string _Name { get => "gpsc"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            string? NAME = GetInputCacheValue("NAME");
            string? PARAMS = GetInputCacheValue("PARAMS");
            string? PASS = GetInputCacheValue("PASS");
            string? SYSFLAGS = GetInputCacheValue("SYSFLAGS");
            string? FORCE_LEAVE = GetInputCacheValue("FORCE_LEAVE");
            string? USERPARAMS = GetInputCacheValue("USERPARAMS");
            string? USERFLAGS = GetInputCacheValue("USERFLAGS");

            if (!string.IsNullOrEmpty(USERPARAMS))
                user.SetParametersFromString(USERPARAMS);
            if (!string.IsNullOrEmpty(USERFLAGS))
                user.Flags = USERFLAGS;

            if (!string.IsNullOrEmpty(FORCE_LEAVE) && FORCE_LEAVE == "1" && user.CurrentGame != null)
            {
                AriesGame prevGame = user.CurrentGame;

                if (prevGame.RemovePlayerByUsername(user.Username))
                    mc.Games.RemoveGame(prevGame);
            }

            if (int.TryParse(GetInputCacheValue("MINSIZE"), out int minSize) && int.TryParse(GetInputCacheValue("MAXSIZE"), out int maxSize)
                && !string.IsNullOrEmpty(PARAMS) && !string.IsNullOrEmpty(NAME) && !string.IsNullOrEmpty(SYSFLAGS)
                && int.TryParse(GetInputCacheValue("PRIV"), out int priv))
            {
                AriesGame? game = mc.Games.AddGame(maxSize, minSize, GetInputCacheValue("CUSTFLAGS"), PARAMS, NAME, priv != 0, GetInputCacheValue("SEED"), SYSFLAGS, PASS, user.CurrentRoom?.ID ?? 0);

                if (game != null)
                {
                    if (game.MinSize > 1 && game.Users.GetUserByName("brobot24") == null)
                        game.AddHost(mc.Users.GetUserByName("brobot24"));

                    if (game.Users.GetUserByName(user.Username) == null)
                        game.AddGPSHost(user);

                    user.CurrentGame = game;

                    client.SendMessage(this);

                    user.SendPlusWho(user, context.Project);

                    game.BroadcastPopulation(mc);
                }
                else
                    client.SendMessage(new GpscDupl());
            }
            else
                client.SendMessage(new GpscInvp());
        }
    }
}
