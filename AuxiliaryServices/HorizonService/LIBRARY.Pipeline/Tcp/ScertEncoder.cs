using CustomLogger;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using Horizon.RT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Horizon.LIBRARY.Pipeline.Tcp
{
    public class ScertEncoder : MessageToMessageEncoder<BaseScertMessage>
    {
        readonly ICipher[] _ciphers = null;
        readonly Func<RT_MSG_TYPE, CipherContext, ICipher> _getCipher = null;

        public ScertEncoder(params ICipher[] ciphers)
        {
            _ciphers = ciphers;
            _getCipher = (id, ctx) =>
            {
                return _ciphers?.FirstOrDefault(x => x.Context == ctx);
            };
        }

        protected override void Encode(IChannelHandlerContext ctx, BaseScertMessage message, List<object> output)
        {
            if (message is null)
                return;

            // Log
            if (message.CanLog())
                LoggerAccessor.LogDebug($"[ScertEncoder] - Tcp: SEND {ctx.Channel}: {message}");

            if (!ctx.HasAttribute(Constants.SCERT_CLIENT))
                ctx.GetAttribute(Constants.SCERT_CLIENT).Set(new Attribute.ScertClientAttribute());
            Attribute.ScertClientAttribute scertClient = ctx.GetAttribute(Constants.SCERT_CLIENT).Get();

            scertClient.OnMessage(message);

            // Serialize
            foreach (byte[] msg in message.Serialize(scertClient.MediusVersion, scertClient.ApplicationID, scertClient.CipherService))
            {
                IByteBuffer byteBuffer = ctx.Allocator.Buffer(msg.Length);
                byteBuffer.WriteBytes(msg);
                output.Add(byteBuffer);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LoggerAccessor.LogError($"[ScertEncoder] - Tcp: An assertion was caught. (Exception:{exception})");
            _ = context.CloseAsync();
        }
    }
}
