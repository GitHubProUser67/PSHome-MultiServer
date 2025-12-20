using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.PolicyResponse)]
    public class MediusGetPolicyResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.PolicyResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public string Policy; // POLICY_MAXLEN
        public bool EndOfText;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            Policy = reader.ReadString(Constants.POLICY_MAXLEN);
            EndOfText = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(Policy, Constants.POLICY_MAXLEN);
            writer.Write(EndOfText);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"Policy: {Policy} " +
                $"EndOfText: {EndOfText}";
        }
    }
}
