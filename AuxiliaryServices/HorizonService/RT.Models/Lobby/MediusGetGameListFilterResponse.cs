using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GetGameListFilterResponse)]
    public class MediusGetGameListFilterResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GetGameListFilterResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public uint FilterID;
        public MediusGameListFilterField FilterField;
        public int Mask;
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
            FilterID = reader.ReadUInt32();
            FilterField = reader.Read<MediusGameListFilterField>();
            Mask = reader.ReadInt32();
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
            writer.Write(FilterID);
            writer.Write(FilterField);
            writer.Write(Mask);
            writer.Write(ComparisonOperator);
            writer.Write(BaselineValue);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
             $"StatusCode:{StatusCode} " +
$"FilterID:{FilterID} " +
$"FilterField:{FilterField} " +
$"Mask:{Mask} " +
$"ComparisonOperator:{ComparisonOperator} " +
$"BaselineValue:{BaselineValue} " +
$"EndOfList:{EndOfList}";
        }
    }
}
