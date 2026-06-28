using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

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
