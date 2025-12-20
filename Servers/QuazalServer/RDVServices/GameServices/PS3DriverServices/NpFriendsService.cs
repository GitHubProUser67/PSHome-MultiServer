using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    [RMCService((ushort)RMCProtocolId.NpFriendsService)]
    public class NpFriendsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult GetNpFriends()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

    }
}
