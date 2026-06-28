using System.Net;
using CustomLogger;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using MultiServerLibrary.Extension.LinqSQL;

namespace Horizon.LIBRARY.Pipeline.Udp
{
    public class ScertDatagramIEnumerableEncoder
        : MessageToMessageEncoder<IEnumerable<ScertDatagramPacket>>
    {
        readonly int maxPacketLength;

        public ScertDatagramIEnumerableEncoder(int maxPacketLengthLocal)
        {
            maxPacketLength = maxPacketLengthLocal;
        }

        protected override void Encode(
            IChannelHandlerContext ctx,
            IEnumerable<ScertDatagramPacket> messages,
            List<object> output
        )
        {
            if (messages is null)
                return;

            var temp = new List<byte[]>();
            var msgsByEndpoint = new Dictionary<EndPoint, List<byte[]>>();

            if (!ctx.HasAttribute(Constants.SCERT_CLIENT))
                ctx.GetAttribute(Constants.SCERT_CLIENT).Set(new Attribute.ScertClientAttribute());
            var scertClient = ctx.GetAttribute(Constants.SCERT_CLIENT).Get();

            // Serialize and add
            foreach (var msg in messages)
            {
                if (
                    msg.Destination != null
                    && !msgsByEndpoint.TryGetValue(msg.Destination, out temp)
                )
                    msgsByEndpoint.Add(msg.Destination, temp = new List<byte[]>());

                if (msg.Message != null)
                    temp.AddRange(
                        msg.Message.Serialize(
                            scertClient.MediusVersion,
                            scertClient.ApplicationID,
                            scertClient.CipherService
                        )
                    );
            }

            foreach (var kvp in msgsByEndpoint)
            {
                // Condense as much as possible
                foreach (
                    var msgGroup in kvp.Value.GroupWhileAggregating(
                        0,
                        (sum, item) => sum + item.Length,
                        (sum, item) => sum < maxPacketLength
                    )
                )
                {
                    var byteBuffer = ctx.Allocator.Buffer(msgGroup.Sum(x => x.Length));
                    foreach (var msg in msgGroup)
                        byteBuffer.WriteBytes(msg);
                    output.Add(new DatagramPacket(byteBuffer, kvp.Key));
                }
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LoggerAccessor.LogError(
                $"[ScertDatagramIEnumerableEncoder] - Udp: An assertion was caught. (Exception:{exception})"
            );
            _ = context.CloseAsync();
        }
    }
}
