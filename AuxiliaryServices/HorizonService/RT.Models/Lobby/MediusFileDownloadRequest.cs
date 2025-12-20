using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileDownload)]
    public class MediusFileDownloadRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileDownload;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// MediusFile requested to download
        /// </summary>
        public MediusFile MediusFileInfo;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MediusFileInfo = reader.Read<MediusFile>();

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            // 
            writer.Write(MediusFileInfo);

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
