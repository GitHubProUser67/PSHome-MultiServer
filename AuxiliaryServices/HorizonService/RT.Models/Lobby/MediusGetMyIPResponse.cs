using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Net;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Response for the request to get your external IP Address
    /// </summary>
	[MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetMyIPResponse)]
    public class MediusGetMyIPResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetMyIPResponse;

        public bool IsSuccess => StatusCode >= 0;
        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Retrieves local IP Address (as seen by the Medius Servers, not behind a NAT).
        /// </summary>
        public IPAddress IP = IPAddress.Any;
        /// <summary>
        /// 
        /// </summary>
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            IP = IPAddress.Parse(reader.ReadString(Constants.IP_MAXLEN));
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(IP?.MapToIPv4()?.ToString(), Constants.IP_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"IP: {IP} " +
                $"StatusCode: {StatusCode}";
        }
    }
}
