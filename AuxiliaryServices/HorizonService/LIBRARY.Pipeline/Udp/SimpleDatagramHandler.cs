using CustomLogger;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;

namespace Horizon.LIBRARY.Pipeline.Udp
{
    public class SimpleDatagramHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        public override bool IsSharable => true;


        public Action<IChannel> OnChannelActive;
        public Action<IChannel> OnChannelInactive;
        public Action<IChannel, DatagramPacket> OnChannelMessage;

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            // Detect when client disconnects
            ctx.Channel.CloseCompletion.ContinueWith((x) =>
            {
                LoggerAccessor.LogWarn("[SimpleDatagramHandler] - Udp: Channel Closed");
                OnChannelInactive?.Invoke(ctx.Channel);
            });

            // Send event upstream
            OnChannelActive?.Invoke(ctx.Channel);
        }

        // The Channel is closed hence the connection is closed
        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            LoggerAccessor.LogWarn("[SimpleDatagramHandler] - Udp: Client disconnected");

            // Send event upstream
            OnChannelInactive?.Invoke(ctx.Channel);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket message)
        {
            // Send upstream
            OnChannelMessage?.Invoke(ctx.Channel, message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LoggerAccessor.LogError($"[SimpleDatagramHandler] - Udp: An assertion was caught. (Exception:{exception})");
        }
    }
}
