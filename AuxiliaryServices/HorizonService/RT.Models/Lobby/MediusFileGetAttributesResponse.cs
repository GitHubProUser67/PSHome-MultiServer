using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileGetAttributesResponse)]
    public class MediusFileGetAttributesResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.FileGetAttributesResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MediusFile MediusFileInfo;
        public MediusFileAttributes MediusFileAttributesResponse;
        public MediusCallbackStatus StatusCode;
        public MessageId MessageID { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MediusFileInfo = reader.Read<MediusFile>();
            MediusFileAttributesResponse = reader.Read<MediusFileAttributes>();

            StatusCode = reader.Read<MediusCallbackStatus>();
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MediusFileInfo);
            writer.Write(MediusFileAttributesResponse);

            writer.Write(StatusCode);
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
             $"MediusFileInfo: {MediusFileInfo}" +
             $"MediusFileAttributesResponse: {MediusFileAttributesResponse} " +
             $"StatusCode: {StatusCode} " +
             $"MessageID: {MessageID} ";
        }
    }
}
