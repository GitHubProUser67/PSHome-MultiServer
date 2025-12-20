using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.LadderList)]
    public class MediusLadderListRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.LadderList;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN

        public int StartByte;
        public int EndByte;
        public int SortOrder;
        public int StartPosition; //Start Sosition Socom: 8, Others 4
        public int PageSize;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            StartByte = reader.ReadInt32();
            EndByte = reader.ReadInt32();
            SortOrder = reader.ReadInt32();
            StartPosition = reader.ReadInt32();
            PageSize = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey);
            writer.Write(2);
            writer.Write(StartByte);
            writer.Write(EndByte);
            writer.Write(SortOrder);
            writer.Write(StartPosition);
            writer.Write(PageSize);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"StartByte: {StartByte} " +
                $"EndByte: {EndByte} " +
                $"SortOrder: {SortOrder} " +
                $"StartPosition: {StartPosition} " +
                $"PageSize: {PageSize} ";

        }
    }
}
