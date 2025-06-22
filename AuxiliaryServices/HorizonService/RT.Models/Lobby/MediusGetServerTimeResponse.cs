using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common;
using Horizon.LIBRARY.Common.Stream;
using NetworkLibrary.Extension;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GetServerTimeResponse)]
    public class MediusGetServerTimeResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GetServerTimeResponse;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int GMT_time = (int)DateTimeUtils.GetUnixTimeU32();
        public MediusTimeZone Local_server_timezone;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            GMT_time = reader.ReadInt32();
            Local_server_timezone = reader.Read<MediusTimeZone>();
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
            writer.Write(GMT_time);
            writer.Write(Local_server_timezone);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode:{StatusCode} " +
                $"GMT_time: {GMT_time} " +
                $"Local_server_timezone: {Local_server_timezone}";
        }
    }
}
