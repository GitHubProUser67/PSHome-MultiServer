using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GetBuddyInvitations)]
    public class MediusGetBuddyInvitationsRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GetBuddyInvitations;

        public MessageId MessageID { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} ";
        }
    }
}
