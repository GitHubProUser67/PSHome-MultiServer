using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class SeleIn : AbstractMessage
    {
        public override string _Name { get => "sele"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;
            AriesUser? user = client.User;

            if (!string.IsNullOrEmpty(context.Project) && (context.Project.Contains("BURNOUT5") || context.Project.Contains("DPR-09") || context.Project.Contains("MOH2") || (context.Project.Contains("NASCAR09") && context.SKU == "PS3")))
            {
                string? STATS = GetInputCacheValue("STATS");
                string? INGAME = GetInputCacheValue("INGAME");
                string? ASYNC = GetInputCacheValue("ASYNC");

                if (STATS == null && INGAME == null)
                    client.SendMessage(new SeleOut()
                    {
                        MORE = "0",
                        SLOTS = "4",
                        STATS = "0"
                    });
                else
                {
                    if ("1".Equals(INGAME))
                    {
                        Dictionary<string, string?> OutCache = new();
                        SeleOut sele = new SeleOut()
                        {
                            INGAME = INGAME,
                            MESGS = GetInputCacheValue("MESGS") ?? "1",
                            MESGTYPES = GetInputCacheValue("MESGTYPES") ?? "P",
                            USERS = GetInputCacheValue("USERS") ?? "0",
                            GAMES = GetInputCacheValue("GAMES") ?? "0",
                            MYGAME = GetInputCacheValue("MYGAME") ?? "0",
                            ROOMS = GetInputCacheValue("ROOMS") ?? "0",
                            ASYNC = ASYNC ?? "0",
                            USERSETS = GetInputCacheValue("USERSETS") ?? "0",
                            STATS = STATS
                        };

                        if (context.Project.Contains("BURNOUT5"))
                        {
                            if (context.SKU == "PS3")
                                OutCache.Add("DP", "PS3/Burnout-Dec2007/mod");
                            else if (context.SKU == "PC")
                            {
                                OutCache.Add("DP", "PC/Burnout-2008/na1");
                                OutCache.Add("GFID", "\"ODS:19038.110.Base Product;BURNOUT PARADISE ULTIMATE EDITION_PC_ONLINE_ACCESS\"");
                                OutCache.Add("PLATFORM", "pc");
                                OutCache.Add("PSID", "PS-REG-BURNOUT2008");
                            }
                            else
                                OutCache.Add("DP", "XBOX360/Burnout-Dec2007/mod");
                            sele.CTRL = "0";
                            sele.SLOTS = "280";
                        }

                        sele.OutputCache = OutCache;

                        client.SendMessage(sele);
                    }
                    else
                        client.SendMessage(new SeleOut()
                        {
                            INGAME = INGAME
                        });
                }

                if (user != null)
                {
                    if (STATS != null || INGAME != null)
                        user.SendPlusWho(user, context.Project);
                    if (user.GetIsGameHost())
                        mc.Rooms.SendRooms(user);
                }
            }
            else
                client.SendMessage(new SeleOut()
                {
                    GAMES = GetInputCacheValue("GAMES") ?? "1",
                    ROOMS = GetInputCacheValue("ROOMS") ?? "1",
                    USERS = GetInputCacheValue("USERS") ?? "1",
                    MESGS = GetInputCacheValue("MESGS") ?? "1",
                    RANKS = GetInputCacheValue("RANKS") ?? "0",
                    MORE = "1",
                    SLOTS = "36"
                });
        }
    }
}
