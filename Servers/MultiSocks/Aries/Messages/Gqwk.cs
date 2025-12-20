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
            string? MODE = GetInputCacheValue("MODE");

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

            if (string.IsNullOrEmpty(MODE) || MODE == "2")
            {
                client.QuickJoinTaskTokenSource = new CancellationTokenSource();
                var token = client.QuickJoinTaskTokenSource.Token;

                client.QuickJoinTask = Task.Run(() => {

                    byte retry = 4;
                    IEnumerable<AriesGame> MatchingList;

                    while (!token.IsCancellationRequested && !client.Disconnected && retry > 0)
                    {
                        MatchingList = mc.Games.GamesSessions.Values.Where(game => !game.Started && !game.Priv && string.IsNullOrEmpty(game.pass) && (game.Users?.Count()) < game.MaxSize);

                        // A handfull of games does custom filtering on top for specific lobbies fetching.
                        if ("BURNOUT5".Equals(context.Project))
                        {
                            List<AriesGame> filteredBurnoutGames = new List<AriesGame>();

                            foreach (var game in MatchingList)
                            {
                                // Friends only.
                                if (game.GPSHost != null && game.MatchesCustFlags("1", "1") && user.Friends.Contains(game.GPSHost.Username))
                                    filteredBurnoutGames.Add(game);
                                // Not private.
                                else if (!game.MatchesCustFlags("2", "2"))
                                    filteredBurnoutGames.Add(game);
                            }

                            MatchingList = filteredBurnoutGames;
                        }

                        AriesGame? filteredGame = MatchingList.FirstOrDefault();

                        if (filteredGame != null)
                        {
                            filteredGame.AddUser(user);

                            user.CurrentGame = filteredGame;

                            client.SendMessage(filteredGame.GetGameDetails(_Name));

                            user.SendPlusWho(user, context.Project);

                            filteredGame.BroadcastPopulation(mc);

                            return;
                        }
						
					    if (retry == 1)
                            client.SendMessage(new MissOut(_Name));

                        retry--;

                        Thread.Sleep(2000);
                    }
                }, token);
            }
            else if (MODE == "3")
                client.StopGameQuickSearch();
        }
    }
}
