using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Sent as request to retrieve version string of current connected Medius Server
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.UtilGetTotalGamesFilteredResponse)]
    public class MediusUtilGetTotalGamesFilteredResponse : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.UtilGetTotalGamesFilteredResponse;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Total Games Returned Filtered
        /// </summary>
        public int Total; // SESSIONKEY_MAXLEN
        /// <summary>
        /// CallbackStatus Code
        /// </summary>
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            Total = reader.ReadInt32();
            StatusCode = reader.Read<MediusCallbackStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(Total);
            writer.Write(StatusCode);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Total: {Total} " +
                $"StatusCode: {StatusCode}";
        }
    }
}
