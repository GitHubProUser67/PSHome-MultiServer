using MultiServerLibrary.Extension.NET;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    [RMCService((ushort)RMCProtocolId.DriverUniqueIDService)]
    public class DriverUniqueIDService : RMCServiceBase
    {
        static readonly UniqueIDGenerator UniqueIDCounter = new(26435);

        [RMCMethod(2)]
        public RMCResult CreateUniqueID()
        {
            return Result(new { value = UniqueIDCounter.CreateSequentialID() });
        }
    }
}
