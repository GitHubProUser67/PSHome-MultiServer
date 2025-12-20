using DotNetty.Transport.Channels;
using Horizon.RT.Models;
using Horizon.MUM.Models;

namespace Horizon.MUIS.PluginArgs
{
    public class OnMessageArgs
    {
        public ClientObject? Player { get; set; } = null;

        public IChannel? Channel { get; set; } = null;

        public BaseScertMessage? Message { get; set; } = null;
        public bool IsIncoming { get; }

        public bool Ignore { get; set; } = false;

        public OnMessageArgs(bool isIncoming)
        {
            IsIncoming = isIncoming;
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Player:{Player} " +
                $"Channel:{Channel} " +
                $"Message:{Message} " +
                $"Ignore:{Ignore}";
        }
    }
}