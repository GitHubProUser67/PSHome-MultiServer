using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.UniverseStatusListResponse)]
    public class MediusUniverseStatusListResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.UniverseStatusListResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public string UniverseName; // UNIVERSENAME_MAXLEN
        public string DNS; // UNIVERSEDNS_MAXLEN
        public int Port;
        public string UniverseDescription;
        public int Status;
        public int UserCount;
        public int MaxUsers;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);

            StatusCode = reader.Read<MediusCallbackStatus>();
            UniverseName = reader.ReadString(Constants.UNIVERSENAME_MAXLEN);
            DNS = reader.ReadString(Constants.UNIVERSEDNS_MAXLEN);
            Port = reader.ReadInt32();
            UniverseDescription = reader.ReadString(Constants.UNIVERSEDESCRIPTION_MAXLEN);
            Status = reader.ReadInt32();
            UserCount = reader.ReadInt32();
            MaxUsers = reader.ReadInt32();
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);

            writer.Write(StatusCode);
            writer.Write(UniverseName, Constants.UNIVERSENAME_MAXLEN);
            writer.Write(DNS, Constants.UNIVERSEDNS_MAXLEN);
            writer.Write(Port);
            writer.Write(UniverseDescription, Constants.UNIVERSEDESCRIPTION_MAXLEN);
            writer.Write(Status);
            writer.Write(UserCount);
            writer.Write(MaxUsers);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"UniverseName: {UniverseName} " +
                $"DNS: {DNS} " +
                $"Port: {Port} " +
                $"UniverseDescription: {UniverseDescription} " +
                $"Status: {Status} " +
                $"UserCount: {UserCount} " +
                $"MaxUsers: {MaxUsers} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
