using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileGetAttributes)]
    public class MediusFileGetAttributesRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileGetAttributes;

        public MessageId MessageID { get; set; }

        public MediusFile MediusFileInfo;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MediusFileInfo = reader.Read<MediusFile>();

            MessageID = reader.Read<MessageId>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MediusFileInfo);

            writer.Write(MessageID ?? MessageId.Empty);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
             $"MediusFileInfo: {MediusFileInfo}" +
             $"MessageID: {MessageID} ";
        }
    }
}
