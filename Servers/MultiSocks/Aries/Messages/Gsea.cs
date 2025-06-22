using MultiSocks.Aries.Model;
using System;

namespace MultiSocks.Aries.Messages
{
    public class Gsea : AbstractMessage
    {
        public override string _Name { get => "gsea"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            if ("1".Equals(GetInputCacheValue("CANCEL")))
            {
                client.CanAsyncGameSearch = false;

                client.SendMessage(this);
                return;
            }
            else if ("1".Equals(GetInputCacheValue("ASYNC")))
                client.CanAsyncGameSearch = true;

            if (int.TryParse(GetInputCacheValue("START"), out int start) && int.TryParse(GetInputCacheValue("COUNT"), out int count))
            {
                string? PLAYERS = GetInputCacheValue("PLAYERS");

                IEnumerable<AriesGame> MatchingList = mc.Games.GamesSessions.Values
                    .Where(game => !game.Started &&  game.MatchesSysFlags(GetInputCacheValue("SYSFLAGS"), GetInputCacheValue("SYSMASK")) && game.MatchesCustFlags(GetInputCacheValue("CUSTFLAGS"), GetInputCacheValue("CUSTMASK")))
                    .Skip(start - 1) // Adjusting for 1-based indexing
                    .Take(count);

                if (!string.IsNullOrEmpty(PLAYERS) && int.TryParse(PLAYERS, out int numOfInPlayers))
                    MatchingList = MatchingList.Where(game => (game.Users?.Count() + numOfInPlayers) <= game.MaxSize);

                if (!string.IsNullOrEmpty(context.Project))
                {
                    // A handfull of games does custom filtering on top for specific lobbies fetching.

                    if (context.Project.Equals("DPR-09"))
                    {
                        string? LANG = GetInputCacheValue("LANG");
                        if (!string.IsNullOrEmpty(LANG) && LANG != "-1")
                            MatchingList = MatchingList.Where(game => game.Params.Contains($"LANG%3d{LANG}") && game.Params.Contains($"VER%3d{GetInputCacheValue("VER")}"));
                    }
                    else if (context.Project.Equals("NASCAR09") && context.SKU == "PS3")
                    {
                        List<AriesGame> filteredNascarGames = new List<AriesGame>();

                        foreach (var game in MatchingList.Where(game => game.Params.Contains($"DNF%3d{GetInputCacheValue("DNF")}")
                        && game.Params.Contains($"MIN_LEVEL%3d{GetInputCacheValue("MIN_LEVEL")}")
                        && game.Params.Contains($"MAX_LEVEL%3d{GetInputCacheValue("MAX_LEVEL")}")))
                        {
                            const string key = "GS=";

                            bool match = true;
                            string gameParams = game.Params;

                            int startIndex = gameParams.IndexOf(key);
                            if (startIndex != -1)
                            {
                                string[] gameGSParams = new string[22];
                                string[] clientGSParams = new string[22];

                                startIndex += key.Length;
                                int endIndex = gameParams.IndexOf('\n', startIndex);
                                if (endIndex == -1) endIndex = gameParams.Length; // in case it's at the end

                                string[] split1 = gameParams.Substring(startIndex, endIndex - startIndex).Split(',');
                                string[] split2 = (GetInputCacheValue("GS") ?? string.Empty).Split(',');

                                for (int i = 0; i < 22; i++)
                                {
                                    if (i < split1.Length) gameGSParams[i] = split1[i];
                                    if (i < split2.Length) clientGSParams[i] = split2[i];
                                }

                                for (int i = 0; i < 22; i++)
                                {
                                    if (!string.IsNullOrEmpty(clientGSParams[i]) && clientGSParams[i] != "-1")
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

                AriesGame[] MatchingArray = MatchingList.ToArray();

                OutputCache.Add("COUNT", MatchingArray.Length.ToString());

                client.SendMessage(this);

                foreach (AriesGame game in MatchingArray)
                {
                    client.SendMessage(game.GetGameDetails("+gam"));
                }
            }
        }
    }
}
