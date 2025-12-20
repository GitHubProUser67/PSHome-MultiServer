using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GenericChatFwdMessage)]
    public class MediusGenericChatFwdMessage : BaseLobbyExtMessage
    {


        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GenericChatFwdMessage;

        public uint TimeStamp;
        public int OriginatorAccountID;
        public MediusChatMessageType MessageType;
        public string OriginatorAccountName; // ACCOUNTNAME_MAXLEN
        public string Message; // CHATMESSAGE_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            TimeStamp = reader.ReadUInt32();
            OriginatorAccountID = reader.ReadInt32();
            MessageType = reader.Read<MediusChatMessageType>();
            OriginatorAccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            Message = reader.ReadString(Constants.CHATMESSAGE_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(TimeStamp);
            writer.Write(OriginatorAccountID);
            writer.Write(MessageType);
            writer.Write(OriginatorAccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(Message, Constants.CHATMESSAGE_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
             $"TimeStamp:{TimeStamp} " +
$"OriginatorAccountID:{OriginatorAccountID} " +
$"MessageType:{MessageType} " +
$"OriginatorAccountName:{OriginatorAccountName} " +
$"Message:{Message}";
        }
    }
}
