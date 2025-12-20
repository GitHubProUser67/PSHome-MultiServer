using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Introduced in Medius API v2.7.<br/>
    /// Allow an application to post ascii information about a <br/>
    /// problem that occurred during online gameplay.This function is strictly used <br/>
    /// only during development, QA and Public Beta phases of a title. <br/> In general, an 
    /// application should not ship with calls to this function.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.PostDebugInfo)]
    public class MediusPostDebugInfoRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.PostDebugInfo;

        public MessageId MessageID { get; set; }
        public string Message; // DEBUGMESSAGE_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            Message = reader.ReadString(Constants.DEBUGMESSAGE_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID);
            writer.Write(Message, Constants.DEBUGMESSAGE_MAXLEN);
        }
    }
}
