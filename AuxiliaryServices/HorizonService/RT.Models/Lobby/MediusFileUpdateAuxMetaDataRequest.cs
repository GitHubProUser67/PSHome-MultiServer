using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.FileUpdateAuxMetaDataRequest)]
    public class MediusFileUpdateAuxMetaDataRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.FileUpdateAuxMetaDataRequest;

        public MessageId MessageID { get; set; }

        public MediusFile MediusFileInfo;
        public MediusFileMetaData MediusUpdateMetaData;
        public ushort Unk1;
        public byte Unk2;

        public override void Deserialize(MessageReader reader)
        {
            // 
            MediusFileInfo = reader.Read<MediusFile>();
            MediusUpdateMetaData = reader.Read<MediusFileMetaData>();

            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
            Unk1 = reader.Read<ushort>();
            Unk2 = reader.ReadByte();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            writer.Write(MediusFileInfo);
            writer.Write(MediusUpdateMetaData);

            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(Unk1);
            writer.Write(Unk2);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"MediusFileInfo: {MediusFileInfo} "  +
                $"MediusUpdateMetaData: {MediusUpdateMetaData} " +
                $"Unk1: {Unk1} " +
                $"Unk2: {Unk2}";
        }
    }
}
