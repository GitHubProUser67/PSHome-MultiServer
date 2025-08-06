using MultiSocks.Aries.Messages.AuthService.News.Burnout;
using MultiSocks.Aries.Messages.AuthService.News.NCAA06;
using System.Text;

namespace MultiSocks.Aries.Messages.AuthService.News
{
    public class NewsIn : AbstractMessage
    {
        public override string _Name { get => "news"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer) return;

            string? NAME = GetInputCacheValue("NAME");

            if (NAME == "7" || NAME == "client.cfg")
            {
                if (!string.IsNullOrEmpty(context.Project) && context.Project.Contains("BURNOUT5"))
                {
                    if (!string.IsNullOrEmpty(context.SKU) && context.SKU.Equals("PS3"))
                    {
                        client.SendMessage(new BOPNewsOut()
                        {
                            BUDDY_SERVER = context.listenIP,
                            QOS_LOBBY = context.listenIP,
                            GPS_REGIONS = $"{context.listenIP},{context.listenIP},{context.listenIP},{context.listenIP}",
                            NEWS_URL = "\"http://gos.ea.com/easo/editorial/common/2008/news/news.jsp?lang=%s&from=%s&game=Burnout&platform=ps3&new=true\"",
                            LIVE_NEWS_URL = !MultiSocksServerConfiguration.RPCS3Workarounds ? "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s\"" : null, // RPCS3 Emulator not emulate the webbrowser (causes the game to be stuck at boot).
                            LIVE_NEWS2_URL = "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s\"",
                            STORE_DLC_URL = "\"http://pctrial.burnoutweb.ea.com/pcstore/store_dlc.php?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s&prodid=%s\"",
                            STORE_URL = "\"http://pctrial.burnoutweb.ea.com/t2b/page/index.php?lang=%s&from=%s&game=Burnout&platform=ps3&env=live&nToken=%s\"",
                            TOSAC_URL = "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=accept&lang=%s&platform=ps3&from=%s\"",
                            TOSA_URL = "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=view&lang=%s&platform=ps3&from=%s\"",
                            TOS_URL = "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?lang=%s&platform=ps3&from=%s\""
                        });
                    }
                    else
                    {
                        client.SendMessage(new BOPNewsOut()
                        {
                            BUDDY_SERVER = context.listenIP,
                            QOS_LOBBY = context.listenIP,
                            GPS_REGIONS = $"{context.listenIP},{context.listenIP},{context.listenIP},{context.listenIP}",
                            NEWS_URL = "\"http://gos.ea.com/easo/editorial/common/2008/news/news.jsp?lang=%s&from=%s&game=Burnout&platform=pc&new=true\"",
                            LIVE_NEWS_URL = "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s\"",
                            LIVE_NEWS2_URL = "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/main.jsp?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s\"",
                            STORE_DLC_URL = "\"http://pctrial.burnoutweb.ea.com/pcstore/store_dlc.php?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s&prodid=%s\"",
                            STORE_URL = "\"http://pctrial.burnoutweb.ea.com/t2b/page/index.php?lang=%s&from=%s&game=Burnout&platform=pc&env=live&nToken=%s\"",
                            TOSAC_URL = "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=accept&lang=%s&platform=pc&from=%s\"",
                            TOSA_URL = "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?style=view&lang=%s&platform=pc&from=%s\"",
                            TOS_URL = "\"http://gos.ea.com/easo/editorial/common/2008/tos/tos.jsp?lang=%s&platform=pc&from=%s\""
                        });
                    }
                }
                else if (!string.IsNullOrEmpty(context.Project) && (context.Project.Contains("PS2/MM06") || context.Project.Contains("MARVEL06")))
                    client.SendMessage(new EASportsNationNewsOut());
                else
                    client.SendMessage(this);
            }
            else if (NAME == "0")
                client.SendMessage(new Newsnew0() { BUDDYRESOURCE = context.Project });
            else if (NAME == "1" || NAME == "3")
                client.SendImmediateMessage(Encoding.ASCII.GetBytes("MultiServer Driven EA Server."));
            else if (NAME == "8")
                client.SendMessage(new Newsnew8());
            // NCAA MM 06, Madden NFL 06
            else if (NAME.Contains("quickmsgs"))
                client.SendMessage(this);
            else if (NAME.Contains("webconfig."))
                client.SendMessage(new WebConfigNewsOut() { BILLBOARD_URL = "http://gos.ea.com/easo/", BILLBOARD_TEXT = "Test" });
            else
            {
                CustomLogger.LoggerAccessor.LogWarn($"[DirtySocks] - News - Client Requested an unknown config type: {NAME}, bouncing...");
                client.SendMessage(this);
            }
        }
    }
}
