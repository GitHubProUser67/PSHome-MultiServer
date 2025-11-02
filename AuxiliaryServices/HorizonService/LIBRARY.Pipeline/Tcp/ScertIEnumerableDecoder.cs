using CustomLogger;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Horizon.LIBRARY.Pipeline.Tcp
{
    public class ScertIEnumerableDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        readonly ICipher[] _ciphers = null;
        readonly Func<RT_MSG_TYPE, CipherContext, ICipher> _getCipher = null;

        /// <summary>
        ///     Create a new instance.
        /// </summary>
        public ScertIEnumerableDecoder(params ICipher[] ciphers)
        {
            _ciphers = ciphers;
            _getCipher = (id, ctx) =>
            {
                return _ciphers?.FirstOrDefault(x => x.Context == ctx);
            };
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                List<object> decoded = Decode(context, input);
                if (decoded != null)
                    output.AddRange(decoded);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn($"[ScertIEnumerableDecoder] - Tcp: Failed to decode a SCERT message. (Exception:{ex})");
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
        protected virtual List<object> Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            List<object> messages = new List<object>();

            byte id = input.GetByte(input.ReaderIndex);
            byte[] hash = null;
            long frameLength = input.GetShortLE(input.ReaderIndex + 1);
            int totalLength = 3;

            if (!context.HasAttribute(Constants.SCERT_CLIENT))
                context.GetAttribute(Constants.SCERT_CLIENT).Set(new Attribute.ScertClientAttribute());
            Attribute.ScertClientAttribute scertClient = context.GetAttribute(Constants.SCERT_CLIENT).Get();

            // only split messages if the RT_MSG_TYPE is the message list id
            if ((id & 0x7F) != 0x3B)
            {
                input.Retain();
                messages.Add(input);
                return messages;
            }

            // empty list, just discard message
            if (frameLength <= 0)
                return null;

            // check if hash is included
            if (id >= 0x80)
            {
                hash = new byte[4];
                input.GetBytes(input.ReaderIndex + 3, hash);
                totalLength += 4;
                id &= 0x7F;
            }

            // never overflows because it's less than maxFrameLength
            int frameLengthInt = (int)frameLength;
            if (input.ReadableBytes < frameLengthInt)
                return null;

            // extract frame
            byte[] messageContents = new byte[frameLengthInt];
            input.GetBytes(input.ReaderIndex + totalLength, messageContents);

            // parse out each message into their own buffers
            for (int i = 0; i < messageContents.Length;)
            {
                int subLen = BitConverter.ToInt16(!BitConverter.IsLittleEndian ? EndianTools.EndianUtils.ReverseArray(messageContents) : messageContents, i + 1) + 3;
                if (messageContents[i] >= 0x80)
                    subLen += 4;

                messages.Add(input.RetainedSlice(input.ReaderIndex + totalLength + i, subLen));
                i += subLen;
            }

            input.SetReaderIndex(input.ReaderIndex + totalLength + frameLengthInt);
            return messages;
        }
    }
}
