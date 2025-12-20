using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetGameListFilterResponse0)]
    public class MediusGetGameListFilterResponse0 : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetGameListFilterResponse0;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public MediusGameListFilterField FilterField;
        public MediusComparisonOperator ComparisonOperator;
        public int BaselineValue;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            FilterField = reader.Read<MediusGameListFilterField>();
            ComparisonOperator = reader.Read<MediusComparisonOperator>();
            BaselineValue = reader.ReadInt32();
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(FilterField);
            writer.Write(ComparisonOperator);
            writer.Write(BaselineValue);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"FilterField: {FilterField} " +
                $"ComparisonOperator: {ComparisonOperator} " +
                $"BaselineValue: {BaselineValue} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
