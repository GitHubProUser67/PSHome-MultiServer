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
            string? GPS = GetInputCacheValue("GPS");

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

                return;
            }
            else if (!string.IsNullOrEmpty(GPS) && GPS == "1") // VIP Mode.
            {
                // Create a game based on server parameters, I suspect EA master server had a VIP config for each titles.
                if (!string.IsNullOrEmpty(user.Connection?.Context.Project) && user.Connection.Context.Project.Contains("BURNOUT5"))
                {
                    // Start a BP Freeburn session ranked.
                    game = mc.Games.AddGame(9, 2, "413017344", "d003f3c04400408847f18ca81800b80,774a70,656e555347f18ca8", user.Username, false, "10", "262208", null, user.CurrentRoom?.ID ?? 0);

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

                        return;
                    }
                }
                else
                    CustomLogger.LoggerAccessor.LogWarn("[Gqwk] - VIP mode required an extra entry in the server code, responding with miss error code, please report to GITHUB!");
            }

            client.SendMessage(new MissOut(_Name));
        }
    }
}
