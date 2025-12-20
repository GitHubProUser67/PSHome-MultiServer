using DotNetty.Transport.Channels;
using Horizon.RT.Models;
using Horizon.MUM.Models;

namespace Horizon.SERVER.PluginArgs
{
    public class OnMediusMessageArgs
    {
        public ClientObject? Player { get; set; } = null;

        public IChannel? Channel { get; set; } = null;

        public BaseMediusMessage? Message { get; set; } = null;

        public bool IsIncoming { get; }

        public bool Ignore { get; set; } = false;

        public OnMediusMessageArgs(bool isIncoming)
        {
            IsIncoming = isIncoming;
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Player: {Player} " +
                $"Channel: {Channel} " +
                $"Message: {Message} " +
                $"Ignore: {Ignore}";
        }
    }
}