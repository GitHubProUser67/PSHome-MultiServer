using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileUpdateMetaDataResponse)]
    public class MediusFileUpdateMetaDataResponse : BaseLobbyMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileUpdateMetaDataResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MediusFile MediusFile;
        public MediusCallbackStatus StatusCode;
        public MessageId MessageID { get; set; }
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MediusFile = reader.Read<MediusFile>();
            StatusCode = reader.Read<MediusCallbackStatus>();
            MessageID = reader.Read<MessageId>();
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(2);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MediusFile);
            writer.Write(StatusCode);
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(EndOfList);
            writer.Write(new byte[2]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +

                $"MediusFileToList: {MediusFile} " +
                $"StatusCode: {StatusCode} " +
                $"MessageID: {MessageID} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
