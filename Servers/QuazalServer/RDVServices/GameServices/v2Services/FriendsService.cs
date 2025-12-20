using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.v2Services
{
    /// <summary>
    /// User friends service
    /// </summary>
    [RMCService((ushort)RMCProtocolId.FriendsService)]
    public class FriendsService : RMCServiceBase
    {
        [RMCMethod(3)]
        public RMCResult UpdateDetails(uint uiPlayer, uint uiDetails)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(10)]
        public RMCResult GetDetailedList(byte byRelationship, bool bReversed)
        {
            // TODO, relationship means to switch to relationship list, bReversed I assume is order.

            List<FriendData> result = new();

            if (bReversed)
                result.Reverse();

            return Result(result);
        }
    }
}