using DotNetty.Transport.Channels;
using Horizon.RT.Models;
using Horizon.DME.Models;

namespace Horizon.DME.PluginArgs
{
    public class OnMessageArgs
    {
        public DMEObject? Player { get; set; } = null;

        public IChannel? Channel { get; set; } = null;

        public BaseScertMessage? Message { get; set; } = null;

        public bool IsIncoming { get; }

        public bool Ignore { get; set; } = false;


        public OnMessageArgs(bool isIncoming)
        {
            IsIncoming = isIncoming;
        }
    }
}