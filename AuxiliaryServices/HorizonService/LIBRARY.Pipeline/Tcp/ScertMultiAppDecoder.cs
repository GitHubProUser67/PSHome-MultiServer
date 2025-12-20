using CustomLogger;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Horizon.RT.Models;
using System;
using System.Collections.Generic;

namespace Horizon.LIBRARY.Pipeline.Tcp
{
    public class ScertMultiAppDecoder : MessageToMessageDecoder<RT_MSG_CLIENT_MULTI_APP_TOSERVER>
    {
        /// <summary>
        ///     Create a new instance.
        /// </summary>
        public ScertMultiAppDecoder()
        {

        }

        protected override void Decode(IChannelHandlerContext context, RT_MSG_CLIENT_MULTI_APP_TOSERVER input, List<object> output)
        {
            try
            {
                foreach (BaseScertMessage message in input.Messages)
                    output.Add(message);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn($"[ScertMultiAppDecoder] - Tcp: Failed to decode a SCERT message. (Exception:{ex})");
            }
        }
    }
}
