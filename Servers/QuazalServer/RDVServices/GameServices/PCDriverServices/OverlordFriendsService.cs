using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
{
    [RMCService((ushort)RMCProtocolId.OverlordFriendsService)]
    public class OverlordFriendsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult SyncFriends(uint friendType, IEnumerable<string> friends)
        {
            var list = new List<string>();
            return Result(list);
        }
    }
}
