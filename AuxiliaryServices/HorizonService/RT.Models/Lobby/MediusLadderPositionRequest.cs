using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.LadderPosition)]
    public class MediusLadderPositionRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.LadderPosition;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN

        public int StartByte;
        public int EndByte;
        public MediusSortOrder SortOrder;
        public int AccountID;
        public int LadderStatIndex;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);

            //
            StartByte = reader.ReadInt32();
            EndByte = reader.ReadInt32();
            SortOrder = reader.Read<MediusSortOrder>();
            AccountID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey);
            writer.Write(new byte[2]);

            //
            writer.Write(StartByte);
            writer.Write(EndByte);
            writer.Write(SortOrder);
            writer.Write(AccountID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StartByte: {StartByte} " +
                $"EndByte: {EndByte} " +
                $"SortOrder: {SortOrder} " +
                $"AccountID: {AccountID} ";
        }
    }
}
