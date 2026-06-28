using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3UbisoftServices
{
    [RMCService((ushort)RMCProtocolId.OverlordCoreProtocolService)]
    public class OverlordCoreProtocolService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult FetchSettingsFromBackend()
        {
            UNIMPLEMENTED();
            return Error(0);
        }
    }
}
