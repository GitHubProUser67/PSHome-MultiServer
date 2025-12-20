using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;
using System.Linq;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_TOKEN_MESSAGE)]
    public class RT_MSG_SERVER_TOKEN_MESSAGE : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_TOKEN_MESSAGE;

        public List<(RT_TOKEN_MESSAGE_TYPE /* tokenMsgType */, ushort /* TokenID */, ushort /* TokenHost */)> TokenList { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            
        }

        public override void Serialize(MessageWriter writer)
        {
            RT_TOKEN_MESSAGE_TYPE tokenMsgType;

            foreach ((RT_TOKEN_MESSAGE_TYPE, ushort, ushort) tokenEntry in TokenList)
            {
                tokenMsgType = tokenEntry.Item1;

                writer.Write(tokenMsgType);
                if (tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_CLIENT_QUERY
                    || tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_CLIENT_REQUEST
                    || tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_CLIENT_RELEASE
                    || tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_OWNED
                    || tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_GRANTED
                    || tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_RELEASED
                    || tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_FREED)
                {
                    writer.Write(tokenEntry.Item2);

                    if (tokenMsgType == RT_TOKEN_MESSAGE_TYPE.RT_TOKEN_SERVER_OWNED)
                        writer.Write(tokenEntry.Item3);
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                   $"TokenList: [{string.Join(", ", TokenList.Select(token => $"RT_TOKEN_MESSAGE_TYPE: {token.Item1}, TokenID: {token.Item2}, TokenHost: {token.Item3}" ))}]";
        }
    }
}
