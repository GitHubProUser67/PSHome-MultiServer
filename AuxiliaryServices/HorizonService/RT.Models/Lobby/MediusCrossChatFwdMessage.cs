using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.CroxxChatFwdMessage)]
    public class MediusCrossChatFwdMessage : BaseLobbyExtMessage
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.CroxxChatFwdMessage;

        public MessageId MessageID { get; set; }

        public int OriginatorAccountID;
        public int TargetRoutingDmeWorldID;
        public int SourceDmeWorldID;

        public MediusCrossChatMessageType msgType;

        public byte[] Contents;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            //
            OriginatorAccountID = reader.ReadInt32();
            TargetRoutingDmeWorldID = reader.ReadInt32();
            SourceDmeWorldID = reader.ReadInt32();

            msgType = reader.Read<MediusCrossChatMessageType>();
            Contents = reader.ReadRest();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);

            // 
            writer.Write(OriginatorAccountID);
            writer.Write(TargetRoutingDmeWorldID);
            writer.Write(SourceDmeWorldID);

            writer.Write(msgType);
            writer.Write(Contents);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"OriginatorAccountID: {OriginatorAccountID} " +
                $"TargetRoutingDmeWorldID: {TargetRoutingDmeWorldID} " +
                $"SourceDmeWorldID: {SourceDmeWorldID} " +
                $"MsgType: {msgType} " +
                $"Contents: {System.BitConverter.ToString(Contents)}";
        }
    }
}
