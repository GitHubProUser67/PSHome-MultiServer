using CustomLogger;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Horizon.RT.Models;
using System;

namespace Horizon.LIBRARY.Pipeline.Tcp
{
    public class ScertServerHandler : SimpleChannelInboundHandler<BaseScertMessage>
    {
        public override bool IsSharable => true;

        public IChannelGroup Group = null;

        public Action<IChannel> OnChannelActive;
        public Action<IChannel> OnChannelInactive;
        public Action<IChannel, BaseScertMessage> OnChannelMessage;

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            IChannelGroup g = Group;
            if (g == null)
            {
                lock (this)
                {
                    if (Group == null)
                        Group = g = new DefaultChannelGroup(ctx.Executor);
                }
            }
            else
                g = Group;

            // Detect when client disconnects
            ctx.Channel.CloseCompletion.ContinueWith((x) =>
            {
                LoggerAccessor.LogWarn("[ScertServerHandler] - Tcp: Channel Closed");
                g?.Remove(ctx.Channel);
                OnChannelInactive?.Invoke(ctx.Channel);
            });

            // Add to channels list
            g?.Add(ctx.Channel);

            // Send event upstream
            OnChannelActive?.Invoke(ctx.Channel);
        }

        // The Channel is closed hence the connection is closed
        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            IChannelGroup g = Group;

            LoggerAccessor.LogWarn("[ScertServerHandler] - Tcp: Client disconnected");

            // Remove
            g?.Remove(ctx.Channel);

            // Send event upstream
            OnChannelInactive?.Invoke(ctx.Channel);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, BaseScertMessage message)
        {
            // Handle medius version
            var scertClient = ctx.GetAttribute(Constants.SCERT_CLIENT).Get();
            if (scertClient != null && scertClient.OnMessage(message))
                ctx.GetAttribute(Constants.SCERT_CLIENT).Set(scertClient);

            // Send upstream
            OnChannelMessage?.Invoke(ctx.Channel, message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LoggerAccessor.LogError($"[ScertServerHandler] - Tcp: An assertion was caught. (Exception:{exception})");
            _ = context.CloseAsync();
        }
    }
}
