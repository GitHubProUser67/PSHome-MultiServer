using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    /// <summary>
    /// Ubi news service
    /// </summary>
    [RMCService((ushort)RMCProtocolId.NewsService)]
    public class NewsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public void GetChannels()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(2)]
        public void GetChannelsByTypes()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(3)]
        public void GetSubscribableChannels()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(4)]
        public void GetChannelsByIDs()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(5)]
        public void GetSubscribedChannels()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(6)]
        public void SubscribeChannel()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(7)]
        public void UnsubscribeChannel()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(8)]
        public RMCResult GetNewsHeaders(NewsRecipient recipient, uint offset, uint size)
        {
            var plInfo = Context?.Client.PlayerInfo;
            var random = new Random();

            var funNews = new List<string>{
                "Actumcnally",
                $"Hello { plInfo?.Name }! Welcome to Alcatraz server!",
                $"Players online: { NetworkPlayers.Players.Count-1 }",
                "Need Deluxe Edition content? Enter IWantDeluxeCars and IWantDeluxeChallenges in Extras > Exclusive Content menu!",
                "All UPlay Rewards were unlocked for you!",
                "Play Driver 2 in Web Browser at opendriver2.github.io!",
                "Subscribe to VortexStory on YouTube!",
                "Support SoapyMan with coffee!",
            };

            var headers = funNews.Select((x, idx) => new NewsHeader
            {
                m_ID = (uint)idx + 1,
                m_publisherName = "SoapyMan",
                m_title = x,
                m_link = string.Empty,
                m_displayTime = DateTime.UtcNow,
                m_expirationTime = DateTime.UtcNow.AddDays(10),
                m_publicationTime = new DateTime(2000, 10, 12, 13, 0, 0),
                m_publisherPID = Context.Client.sPID,
                m_recipientID = Context.Client.IDsend,
                m_recipientType = 0,
            });

            return Result(headers);
        }

        [RMCMethod(9)]
        public RMCResult GetNewsMessages(IEnumerable<uint> messageIds)
        {
            return Result(new List<string>
            {
                "This text apparently doesn't work."
            });
        }

        [RMCMethod(10)]
        public RMCResult GetNumberOfNews(NewsRecipient recipient)
        {
            return Result(new { number_of_news = 0 });
        }

        [RMCMethod(11)]
        public void GetChannelByType()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(12)]
        public void GetNewsHeadersByType()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(13)]
        public void GetNewsMessagesByType()
        {
            UNIMPLEMENTED();
        }
    }
}
