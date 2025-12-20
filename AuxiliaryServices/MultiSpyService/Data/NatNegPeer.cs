using System.Net;

namespace MultiSpyService.Data
{
    public class NatNegPeer
    {
        public IPEndPoint PublicAddress;
        public IPEndPoint CommunicationAddress;
        public bool IsHost;
    }

    public class NatNegClient
    {
        public int ClientId;
        public NatNegPeer Host;
        public NatNegPeer Guest;
    }
}
