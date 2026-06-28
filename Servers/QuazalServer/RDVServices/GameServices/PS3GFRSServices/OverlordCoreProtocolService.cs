using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
{
    [RMCService((ushort)RMCProtocolId.OverlordCoreProtocolService)]
    public class OverlordCoreProtocolService : RMCServiceBase
    {
        [RMCMethod(1)]
        public void FetchSettingsFromBackend()
        {
            UNIMPLEMENTED();
        }
    }
}
