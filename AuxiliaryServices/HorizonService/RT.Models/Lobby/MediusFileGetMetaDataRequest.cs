using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileGetMetaData)]
    public class MediusFileGetMetaDataRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileGetMetaData;

        public MessageId MessageID { get; set; }

        public MediusFile MediusFileInfo;
        public MediusFileMetaData MediusMetaDataRequestedKey;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MediusFileInfo = reader.Read<MediusFile>();
            MediusMetaDataRequestedKey = reader.Read<MediusFileMetaData>();

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MediusFileInfo);
            writer.Write(MediusMetaDataRequestedKey);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MediusFileInfo: {MediusFileInfo} "  +
                $"MediusDataRequestedKey: {MediusMetaDataRequestedKey}" +
                $"MessageID: {MessageID} ";
        }
    }
}
