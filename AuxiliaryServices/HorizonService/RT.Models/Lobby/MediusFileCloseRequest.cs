using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Introduced in Medius v1.50
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileClose)]
    public class MediusFileCloseRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileClose;

        public MessageId MessageID { get; set; }
        public MediusFile MediusFileInfo = new MediusFile();
        public override void Deserialize(MessageReader reader)
        {
            // 
            MediusFileInfo = reader.Read<MediusFile>();

            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            writer.Write(MediusFileInfo);

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
                $"MediusFileInfo: {MediusFileInfo}";
        }
    }
}
