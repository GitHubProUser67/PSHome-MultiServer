using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Gqwk : AbstractMessage
    {
        public override string _Name { get => "gqwk"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            string? USERPARAMS = GetInputCacheValue("USERPARAMS");
            string? USERFLAGS = GetInputCacheValue("USERFLAGS");
            string? FORCE_LEAVE = GetInputCacheValue("FORCE_LEAVE");

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

            AriesGame? game = mc.Games.GamesSessions.Values.Where(game => !game.Started && !game.Priv && string.IsNullOrEmpty(game.pass) && (game.Users?.Count() + 1) <= game.MaxSize).FirstOrDefault();

            if (game != null)
            {
                game.AddUser(user);

                user.CurrentGame = game;

                client.SendMessage(game.GetGameDetails(_Name));

                user.SendPlusWho(user, context.Project);

                game.BroadcastPopulation(mc);
            }
            else
                client.SendMessage(new MissOut(_Name));
        }
    }
}
