using System.Text;
using MultiSocks.Aries.Components.AuthService.News.Burnout;

namespace MultiSocks.Aries.Components.AuthService.News
{
    public class NewsIn : AbstractMessage
    {
        public override string _Name
        {
            get => "news";
        }

        private const string WebMesg =
            "Welcome! You are now connected to the MultiServer EA system.";

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer)
                return;

            var NAME = GetInputCacheValue("NAME") ?? string.Empty;

            if (NAME.EndsWith(".cfg"))
            {
                var isPs3 = "PS3".Equals(context.SKU);
                var configName = NAME.Split('.')[0];

                switch (configName)
                {
                    case "client":
                        if (!string.IsNullOrEmpty(context.Project))
                        {
                            if (context.Project.Contains("BURNOUT5"))
                            {
                                if (isPs3)
                                {
                                    client.SendMessage(
                                        new BOPNewsOut()
                                        {
                                            NEWS_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/news/news.jsp?lang=%s&from=%s&game=Burnout&platform=ps3&new=true\"",
                                            LIVE_NEWS_URL =
                                                !MultiSocksServerConfiguration.RPCS3Workarounds
                                                    ? "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s\""
                                                    : null, // RPCS3 Emulator not emulate the webbrowser (causes the game to be stuck at boot).
                                            LIVE_NEWS2_URL =
                                                "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s\"",
                                            STORE_DLC_URL =
                                                "\"http://pctrial.burnoutweb.ea.com/pcstore/store_dlc.php?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s&prodid=%s\"",
                                            STORE_URL =
                                                "\"http://pctrial.burnoutweb.ea.com/t2b/page/index.php?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s\"",
                                            TOSAC_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=accept&lang=%s&platform=ps3&from=%s\"",
                                            TOSA_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=view&lang=%s&platform=ps3&from=%s\"",
                                            TOS_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?lang=%s&platform=ps3&from=%s\"",
                                        }
                                    );
                                }
                                else
                                {
                                    client.SendMessage(
                                        new BOPNewsOut()
                                        {
                                            NEWS_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/news/news.jsp?lang=%s&from=%s&game=Burnout&platform=pc&new=true\"",
                                            LIVE_NEWS_URL =
                                                "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s\"",
                                            LIVE_NEWS2_URL =
                                                "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s\"",
                                            STORE_DLC_URL =
                                                "\"http://pctrial.burnoutweb.ea.com/pcstore/store_dlc.php?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s&prodid=%s\"",
                                            STORE_URL =
                                                "\"http://pctrial.burnoutweb.ea.com/t2b/page/index.php?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s\"",
                                            TOSAC_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=accept&lang=%s&platform=pc&from=%s\"",
                                            TOSA_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=view&lang=%s&platform=pc&from=%s\"",
                                            TOS_URL =
                                                "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?lang=%s&platform=pc&from=%s\"",
                                        }
                                    );
                                }

                                return;
                            }
                        }

                        if (isPs3)
                        {
                            client.SendMessage(
                                new DefaultNewsOutCfg()
                                {
                                    NEWS_URL =
                                        $"\"http://gos.ea.com/easo/editorial/common/2008/news/news.jsp?lang=%s&from=%s&game={context.Project}&platform=ps3&new=true\"",
                                    TOSAC_URL =
                                        "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=accept&lang=%s&platform=ps3&from=%s\"",
                                    TOSA_URL =
                                        "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=view&lang=%s&platform=ps3&from=%s\"",
                                    TOS_URL =
                                        "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?lang=%s&platform=ps3&from=%s\"",
                                }
                            );
                        }
                        else
                        {
                            client.SendMessage(
                                new DefaultNewsOutCfg()
                                {
                                    NEWS_URL =
                                        $"\"http://gos.ea.com/easo/editorial/common/2008/news/news.jsp?lang=%s&from=%s&game={context.Project}&platform=pc&new=true\"",
                                    TOSAC_URL =
                                        "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=accept&lang=%s&platform=pc&from=%s\"",
                                    TOSA_URL =
                                        "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=view&lang=%s&platform=pc&from=%s\"",
                                    TOS_URL =
                                        "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?lang=%s&platform=pc&from=%s\"",
                                }
                            );
                        }
                        return;
                    case "nucleus":
                        client.SendMessage(new NucleusNewsOut());
                        return;
                }

                CustomLogger.LoggerAccessor.LogWarn(
                    $"[NewsIn] - Client Requested an unknown config file: {NAME}, bouncing..."
                );
                client.SendMessage(this);
            }
            else if (NAME == "0")
                client.SendMessage(new Newsnew0() { BUDDYRESOURCE = context.Project });
            else if (NAME == "1" || NAME == "3")
                client.SendImmediateMessage(Encoding.ASCII.GetBytes(WebMesg));
            else if (NAME == "7")
            {
                if (
                    !string.IsNullOrEmpty(context.Project)
                    && (
                        context.Project.Contains("PS2/MM06") || context.Project.Contains("MARVEL06")
                    )
                )
                    client.SendMessage(new EASportsNationNewsOut());
                else
                    client.SendMessage(this);
            }
            else if (NAME == "8")
                client.SendMessage(new Newsnew8());
            else
            {
                CustomLogger.LoggerAccessor.LogWarn(
                    $"[NewsIn] - Client Requested an unknown config type: {NAME}, bouncing..."
                );
                client.SendMessage(this);
            }
        }
    }
}
