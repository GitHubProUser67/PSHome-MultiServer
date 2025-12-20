using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models.Lobby
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.JoinLeastPopulatedChannelResponse)]
    public class MediusJoinLeastPopulatedChannelResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.JoinLeastPopulatedChannelResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int MediusWorldID;
        public NetConnectionInfo ConnectInfo;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            StatusCode = reader.Read<MediusCallbackStatus>();
            MediusWorldID = reader.ReadInt32();
            ConnectInfo = reader.Read<NetConnectionInfo>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(StatusCode);
            writer.Write(MediusWorldID);
            writer.Write(ConnectInfo);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"ConnectInfo: {ConnectInfo}";
        }
    }
}
