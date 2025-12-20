using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
{
    [RMCService((ushort)RMCProtocolId.OverlordDareProtocolService)]
    public class OverlordDareProtocolService : RMCServiceBase
    {
        [RMCMethod(1)]
        public void requestUnreadCount()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(2)]
        public void deleteDare()
        {
            UNIMPLEMENTED();
        }
        [RMCMethod(3)]
        public void postDare()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(4)]
        public void requestDares()
        {
            UNIMPLEMENTED();
        }
    }
}
