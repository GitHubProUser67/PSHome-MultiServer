using Horizon.DME.Models;
using Horizon.RT.Models;

namespace Horizon.DME.PluginArgs
{
    public class OnTcpMsg
    {
        public DMEObject? Player { get; set; }

        public BaseScertMessage? Packet { get; set; }

        public bool Ignore { get; set; }

        public bool IsIncoming { get; }

        public OnTcpMsg(bool isIncoming)
        {
            IsIncoming = isIncoming;
        }
    }
}
