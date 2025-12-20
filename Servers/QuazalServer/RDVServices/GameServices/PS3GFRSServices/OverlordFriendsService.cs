using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
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
