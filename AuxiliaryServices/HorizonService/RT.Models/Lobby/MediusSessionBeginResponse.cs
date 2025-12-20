using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.SessionBeginResponse)]
    public class MediusSessionBeginResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.SessionBeginResponse;
        List<int> shortPadding = new List<int> { 10694, 21064 };

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public string SessionKey; // SESSIONKEY_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            if (shortPadding.Contains(reader.AppId))
                reader.ReadBytes(1);
            else
                reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            if (shortPadding.Contains(writer.AppId))
                writer.Write(new byte[1]);
            else
                writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"SessionKey: {SessionKey}";
        }
    }
}
