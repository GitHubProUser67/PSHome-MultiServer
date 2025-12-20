using Horizon.RT.Models;
using Horizon.DME.Models;

namespace Horizon.DME.PluginArgs
{
    public class OnTcpMsg
    {
        public DMEObject? Player { get; set; }

        public BaseScertMessage? Packet { get; set; }

        public bool Ignore { get; set; }
    }
}