using CustomLogger;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Horizon.LIBRARY.Common;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using DotNetty.Buffers;
using MultiServerLibrary.Extension.LinqSQL;

namespace Horizon.LIBRARY.Pipeline.Udp
{
    public class ScertDatagramIEnumerableEncoder : MessageToMessageEncoder<IEnumerable<ScertDatagramPacket>>
    {
        readonly int maxPacketLength;

        public ScertDatagramIEnumerableEncoder(int maxPacketLengthLocal)
        {
            maxPacketLength = maxPacketLengthLocal;
        }

        protected override void Encode(IChannelHandlerContext ctx, IEnumerable<ScertDatagramPacket> messages, List<object> output)
        {
            if (messages is null)
                return;

            List<byte[]> temp = new List<byte[]>();
            Dictionary<EndPoint, List<byte[]>> msgsByEndpoint = new Dictionary<EndPoint, List<byte[]>>();

            if (!ctx.HasAttribute(Constants.SCERT_CLIENT))
                ctx.GetAttribute(Constants.SCERT_CLIENT).Set(new Attribute.ScertClientAttribute());
            Attribute.ScertClientAttribute scertClient = ctx.GetAttribute(Constants.SCERT_CLIENT).Get();

            // Serialize and add
            foreach (ScertDatagramPacket msg in messages)
            {
                if (msg.Destination != null && !msgsByEndpoint.TryGetValue(msg.Destination, out temp))
                    msgsByEndpoint.Add(msg.Destination, temp = new List<byte[]>());

                if (msg.Message != null)
                    temp.AddRange(msg.Message.Serialize(scertClient.MediusVersion, scertClient.ApplicationID, scertClient.CipherService));
            }

            foreach (KeyValuePair<EndPoint, List<byte[]>> kvp in msgsByEndpoint)
            {
                // Condense as much as possible
                foreach (IEnumerable<byte[]> msgGroup in kvp.Value.GroupWhileAggregating(0, (sum, item) => sum + item.Length, (sum, item) => sum < maxPacketLength))
                {
                    IByteBuffer byteBuffer = ctx.Allocator.Buffer(msgGroup.Sum(x => x.Length));
                    foreach (byte[] msg in msgGroup)
                        byteBuffer.WriteBytes(msg);
                    output.Add(new DatagramPacket(byteBuffer, kvp.Key));
                }
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LoggerAccessor.LogError($"[ScertDatagramIEnumerableEncoder] - Udp: An assertion was caught. (Exception:{exception})");
            _ = context.CloseAsync();
        }
    }
}
