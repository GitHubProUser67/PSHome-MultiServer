using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Introduced in 1.50<br></br>
    /// Request to create a file using Medius File Services
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileCreate)]
    public class MediusFileCreateRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileCreate;

        public MessageId MessageID { get; set; }

        public MediusFile MediusFileToCreate;
        public MediusFileAttributes MediusFileCreateAttributes;

        public override void Deserialize(MessageReader reader)
        {
            // 
            MediusFileToCreate = reader.Read<MediusFile>();
            MediusFileCreateAttributes = reader.Read<MediusFileAttributes>();

            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            writer.Write(MediusFileToCreate);
            writer.Write(MediusFileCreateAttributes);

            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"MediusFileToCreate: {MediusFileToCreate} " +
                $"MediusFileCreateAttributes: {MediusFileCreateAttributes}";
        }
    }
}
