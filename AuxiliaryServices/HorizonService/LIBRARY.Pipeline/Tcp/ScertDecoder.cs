using CustomLogger;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Horizon.LIBRARY.Pipeline.Attribute;
using Horizon.RT.Common;
using Horizon.RT.Models;
using System;
using System.Collections.Generic;

namespace Horizon.LIBRARY.Pipeline.Tcp
{
    public class ScertDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        /// <summary>
        ///     Create a new instance.
        /// </summary>
        public ScertDecoder()
        {

        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                object decoded = Decode(context, input);
                if (decoded != null)
                    output.Add(decoded);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn($"[ScertDecoder] - Tcp: Failed to decode a SCERT message. (Exception:{ex})");
            }
        }

        /// <summary>
        ///     Create a frame out of the <see cref="IByteBuffer" /> and return it.
        /// </summary>
        /// <param name="context">
        ///     The <see cref="IChannelHandlerContext" /> which this <see cref="ByteToMessageDecoder" /> belongs
        ///     to.
        /// </param>
        /// <param name="input">The <see cref="IByteBuffer" /> from which to read data.</param>
        /// <returns>The <see cref="IByteBuffer" /> which represents the frame or <c>null</c> if no frame could be created.</returns>
        protected virtual object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            byte id = input.GetByte(input.ReaderIndex);
            byte[] hash = null;
            long frameLength = input.GetShortLE(input.ReaderIndex + 1);
            int totalLength = 3;

            if (!context.HasAttribute(Constants.SCERT_CLIENT))
                context.GetAttribute(Constants.SCERT_CLIENT).Set(new ScertClientAttribute());
            ScertClientAttribute scertClient = context.GetAttribute(Constants.SCERT_CLIENT).Get();

            if (frameLength <= 0)
                return BaseScertMessage.Instantiate((RT_MSG_TYPE)(id & 0x7F), null, Array.Empty<byte>(), scertClient.MediusVersion != null ? (int)scertClient.MediusVersion : 108, scertClient.ApplicationID, scertClient.CipherService);

            if (id >= 0x80)
            {
                hash = new byte[4];
                input.GetBytes(input.ReaderIndex + 3, hash);
                totalLength += 4;
                id &= 0x7F;
            }

            if (frameLength < 0)
                throw new InvalidOperationException("negative pre-adjustment length field: " + frameLength);

            // never overflows because it's less than maxFrameLength
            int frameLengthInt = (int)frameLength;
            if (input.ReadableBytes < frameLengthInt)
                return null;

            // extract frame
            byte[] messageContents = new byte[frameLengthInt];
            input.GetBytes(input.ReaderIndex + totalLength, messageContents);

            input.SetReaderIndex(input.ReaderIndex + totalLength + frameLengthInt);
            return BaseScertMessage.Instantiate((RT_MSG_TYPE)id, hash, messageContents, scertClient.MediusVersion != null ? (int)scertClient.MediusVersion : 108, scertClient.ApplicationID, scertClient.CipherService);
        }
    }
}
