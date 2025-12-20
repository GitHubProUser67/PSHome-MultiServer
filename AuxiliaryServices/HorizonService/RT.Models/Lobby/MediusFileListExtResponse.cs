using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.FileListExtResponse)]
    public class MediusFileListExtResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.FileListExtResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MediusFile MediusFileInfo;
        public string MetaValue;
        public MediusCallbackStatus StatusCode;
        public MessageId MessageID { get; set; }
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MediusFileInfo = reader.Read<MediusFile>();
            MetaValue = reader.ReadString(Constants.MEDIUS_FILE_MAX_VALUE_LENGTH);
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
            writer.Write(MediusFileInfo);
            writer.Write(MetaValue, Constants.MEDIUS_FILE_MAX_VALUE_LENGTH);
            writer.Write(StatusCode);
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(EndOfList);
            writer.Write(new byte[2]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +

                $"MediusFileInfo: {MediusFileInfo} " +
                $"MetaValue: {MetaValue} " +
                $"StatusCode: {StatusCode} " +
                $"MessageID: {MessageID} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
