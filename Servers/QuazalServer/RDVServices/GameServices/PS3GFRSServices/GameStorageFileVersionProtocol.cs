using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
{
    [RMCService((ushort)RMCProtocolId.GameStorageFileVersionProtocol)]
    public class GameStorageFileVersionProtocol : RMCServiceBase
    {
        [RMCMethod(2)]
        public RMCResult GameStorageFileVersion(string FileName)
        {
            UNIMPLEMENTED();
            return Error(0);
        }
    }
}
