using Horizon.DME.Models;
using Horizon.LIBRARY.Pipeline.Udp;

namespace Horizon.DME.PluginArgs
{
    public class OnUdpMsg
    {
        public DMEObject? Player { get; set; }

        public ScertDatagramPacket? Packet { get; set; }

        public bool Ignore { get; set; }

        public bool IsIncoming { get; }

        public OnUdpMsg(bool isIncoming)
        {
            IsIncoming = isIncoming;
        }
    }
}
