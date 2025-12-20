using DotNetty.Transport.Channels;
using Horizon.RT.Models;
using Horizon.DME.Models;

namespace Horizon.DME.PluginArgs
{
    public class OnMediusMessageArgs
    {
        public DMEObject? Player { get; set; } = null;

        public IChannel? Channel { get; set; } = null;

        public BaseMediusMessage? Message { get; set; } = null;

        public bool IsIncoming { get; }

        public bool Ignore { get; set; } = false;

        public OnMediusMessageArgs(bool isIncoming)
        {
            IsIncoming = isIncoming;
        }
    }
}