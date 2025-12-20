using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    [RMCService((ushort)RMCProtocolId.DriverUniqueIDService)]
    public class DriverUniqueIDService : RMCServiceBase
    {
        static UniqueIDGenerator UniqueIDCounter = new UniqueIDGenerator(26434);

        [RMCMethod(2)]
        public RMCResult CreateUniqueID()
        {
            return Result(new { value = UniqueIDCounter.CreateUniqueID() });
        }
    }
}
