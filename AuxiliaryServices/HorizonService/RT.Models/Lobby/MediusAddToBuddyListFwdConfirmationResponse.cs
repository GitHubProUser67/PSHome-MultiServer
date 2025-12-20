using System.IO;
using Horizon.RT.Common;
using Horizon.RT.Models.Misc;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.AddToBuddyListFwdConfirmationResponse)]
    public class MediusAddToBuddyListFwdConfirmationResponse : BaseLobbyMessage, IMediusAddToBuddyListConfirmationResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.AddToBuddyListFwdConfirmationResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public string SessionKey; //SESSIONKEY_MAXLEN
        public MediusCallbackStatus StatusCode;
        public int OriginatorAccountID { get; set; }
        public MediusBuddyAddType AddType { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            StatusCode = reader.Read<MediusCallbackStatus>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);

            OriginatorAccountID = reader.ReadInt32();
            AddType = reader.Read<MediusBuddyAddType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);

            writer.Write(StatusCode);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(OriginatorAccountID);
            writer.Write(AddType);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " + 
                $"SessionKey: {SessionKey} " +
                $"OriginatorAccountID: {OriginatorAccountID} " +
                $"AddType: {AddType} ";
        }
    }
}
