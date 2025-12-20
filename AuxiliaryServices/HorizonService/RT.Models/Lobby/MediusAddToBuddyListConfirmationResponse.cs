using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.AddToBuddyListConfirmationResponse)]
    public class MediusAddToBuddyListConfirmationResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.AddToBuddyListConfirmationResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int TargetAccountID;
        public string TargetAccountName;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            StatusCode = reader.Read<MediusCallbackStatus>();
            TargetAccountID = reader.ReadInt32();
            TargetAccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);

            writer.Write(StatusCode);
            writer.Write(TargetAccountID);
            writer.Write(TargetAccountName, Constants.ACCOUNTNAME_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"TargetAccountID: {TargetAccountID} " +
                $"TargetAccountName: {TargetAccountName}";
        }
    }
}
