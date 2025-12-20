using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ChannelListResponse)]
    public class MediusChannelListResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.ChannelListResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int MediusWorldID;
        public string LobbyName; // LOBBYNAME_MAXLEN
        public int PlayerCount;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            MediusWorldID = reader.ReadInt32();
            LobbyName = reader.ReadString(Constants.LOBBYNAME_MAXLEN);
            PlayerCount = reader.ReadInt32();
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(MediusWorldID);
            writer.Write(LobbyName, Constants.LOBBYNAME_MAXLEN);
            writer.Write(PlayerCount);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"StatusCode:{StatusCode} " +
                $"MediusWorldID:{MediusWorldID} " +
                $"LobbyName:{LobbyName} " +
                $"PlayerCount:{PlayerCount} " +
                $"EndOfList:{EndOfList}";
        }
    }
}
