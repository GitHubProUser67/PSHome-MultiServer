using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Components
{
    public class Gsea : AbstractMessage
    {
        public override string _Name
        {
            get => "gsea";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc)
                return;

            var user = client.User;
            if (user == null)
                return;

            if ("1".Equals(GetInputCacheValue("CANCEL")))
            {
                client.CanAsyncGameSearch = false;

                client.SendMessage(this);
                return;
            }
            else if ("1".Equals(GetInputCacheValue("ASYNC")))
                client.CanAsyncGameSearch = true;

            if (
                int.TryParse(GetInputCacheValue("START"), out var start)
                && int.TryParse(GetInputCacheValue("COUNT"), out var count)
            )
            {
                var PLAYERS = GetInputCacheValue("PLAYERS");

                var MatchingList = mc
                    .Games.GamesSessions.Values.Where(game =>
                        !game.Started
                        && game.MatchesSysFlags(
                            GetInputCacheValue("SYSMASK"),
                            GetInputCacheValue("SYSFLAGS")
                        )
                        && game.MatchesCustFlags(
                            GetInputCacheValue("CUSTMASK"),
                            GetInputCacheValue("CUSTFLAGS")
                        )
                    )
                    .Skip(start - 1) // Adjusting for 1-based indexing
                    .Take(count);

                if (!string.IsNullOrEmpty(PLAYERS) && int.TryParse(PLAYERS, out var numOfInPlayers))
                    MatchingList = MatchingList.Where(game =>
                        (game.GetActiveUsersCount() + numOfInPlayers) <= game.MaxSize
                    );

                if (!string.IsNullOrEmpty(context.Project))
                {
                    // A handfull of games does custom filtering on top for specific lobbies fetching.
                    if ("BURNOUT5".Equals(context.Project))
                    {
                        var filteredBurnoutGames = new List<AriesGame>();

                        foreach (var game in MatchingList)
                        {
                            // Friends only.
                            if (
                                game.GPSHost != null
                                && game.MatchesCustFlags("1", "1")
                                && user.Friends.Contains(game.GPSHost.Username)
                            )
                                filteredBurnoutGames.Add(game);
                            // Not private.
                            else if (!game.MatchesCustFlags("2", "2"))
                                filteredBurnoutGames.Add(game);
                        }

                        MatchingList = filteredBurnoutGames;
                    }
                    else if ("DPR-09".Equals(context.Project))
                    {
                        var LANG = GetInputCacheValue("LANG");
                        if (!string.IsNullOrEmpty(LANG) && LANG != "-1")
                            MatchingList = MatchingList.Where(game =>
                                game.Params.Contains($"LANG%3d{LANG}")
                                && game.Params.Contains($"VER%3d{GetInputCacheValue("VER")}")
                            );
                    }
                    else if ("NASCAR09".Equals(context.Project) && context.SKU == "PS3")
                    {
                        var filteredNascarGames = new List<AriesGame>();

                        foreach (
                            var game in MatchingList.Where(game =>
                                game.Params.Contains($"DNF%3d{GetInputCacheValue("DNF")}")
                                && game.Params.Contains(
                                    $"MIN_LEVEL%3d{GetInputCacheValue("MIN_LEVEL")}"
                                )
                                && game.Params.Contains(
                                    $"MAX_LEVEL%3d{GetInputCacheValue("MAX_LEVEL")}"
                                )
                            )
                        )
                        {
                            const string key = "GS=";

                            var match = true;
                            var gameParams = game.Params;

                            var startIndex = gameParams.IndexOf(key);
                            if (startIndex != -1)
                            {
                                var gameGSParams = new string[22];
                                var clientGSParams = new string[22];

                                startIndex += key.Length;
                                var endIndex = gameParams.IndexOf('\n', startIndex);
                                if (endIndex == -1)
                                    endIndex = gameParams.Length; // in case it's at the end

                                var split1 = gameParams
                                    .Substring(startIndex, endIndex - startIndex)
                                    .Split(',');
                                var split2 = (GetInputCacheValue("GS") ?? string.Empty).Split(',');

                                for (var i = 0; i < 22; i++)
                                {
                                    if (i < split1.Length)
                                        gameGSParams[i] = split1[i];
                                    if (i < split2.Length)
                                        clientGSParams[i] = split2[i];
                                }

                                for (var i = 0; i < 22; i++)
                                {
                                    if (
                                        !string.IsNullOrEmpty(clientGSParams[i])
                                        && clientGSParams[i] != "-1"
                                    )
                                    {
                                        if (gameGSParams[i] != clientGSParams[i])
                                        {
                                            match = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (match)
                                filteredNascarGames.Add(game);
                        }

                        MatchingList = filteredNascarGames;
                    }
                }

                var MatchingArray = MatchingList.ToArray();

                OutputCache.Add("COUNT", MatchingArray.Length.ToString());

                client.SendMessage(this);

                foreach (var game in MatchingArray)
                {
                    client.SendMessage(game.GetGameDetails("+gam"));
                }
            }
        }
    }
}
