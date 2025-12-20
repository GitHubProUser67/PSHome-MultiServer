using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.SetGameListFilter)]
    public class MediusSetGameListFilterRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.SetGameListFilter;

        public MessageId MessageID { get; set; }

        public MediusGameListFilterField FilterField;
        public int Mask;
        public MediusComparisonOperator ComparisonOperator;
        public int BaselineValue;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            FilterField = reader.Read<MediusGameListFilterField>();
            Mask = reader.ReadInt32();
            ComparisonOperator = reader.Read<MediusComparisonOperator>();
            BaselineValue = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(FilterField);
            writer.Write(Mask);
            writer.Write(ComparisonOperator);
            writer.Write(BaselineValue);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"FilterField: {FilterField} " +
                $"Mask: {Mask} " +
                $"ComparisonOperator: {ComparisonOperator} " +
                $"BaselineValue: {BaselineValue}";
        }
    }
}
