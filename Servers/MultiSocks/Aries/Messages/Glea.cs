using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Glea : AbstractMessage
    {
        public override string _Name { get => "glea"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            if (user.CurrentGame != null)
            {
                string? FORCE = GetInputCacheValue("FORCE");

                if ((!string.IsNullOrEmpty(FORCE) && FORCE == "1") || !user.CurrentGame.Started) // Don't quit immediatly if game is started or FORCE is triggered.
                {
                    client.SendMessage(user.CurrentGame.GetGameDetails(_Name));

                    AriesGame prevGame = user.CurrentGame;

                    if (prevGame.RemovePlayerByUsername(user.Username))
                        mc.Games.RemoveGame(prevGame);

                    return;
                }
            }

            client.SendMessage(this);
        }
    }
}
