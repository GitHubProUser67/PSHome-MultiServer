using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.PlayerInfoResponse)]
    public class MediusPlayerInfoResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.PlayerInfoResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public string AccountName; // ACCOUNTNAME_MAXLEN
        public int ApplicationID;
        public MediusPlayerStatus PlayerStatus;
        public MediusConnectionType ConnectionClass;
        public byte[] Stats = new byte[Constants.ACCOUNTSTATS_MAXLEN];

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            AccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            ApplicationID = reader.ReadInt32();
            PlayerStatus = reader.Read<MediusPlayerStatus>();
            ConnectionClass = reader.Read<MediusConnectionType>();
            Stats = reader.ReadBytes(Constants.ACCOUNTSTATS_MAXLEN);
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
            writer.Write(AccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(ApplicationID);
            writer.Write(PlayerStatus);
            writer.Write(ConnectionClass);
            writer.Write(Stats, Constants.ACCOUNTSTATS_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"AccountName: {AccountName} " +
                $"ApplicationID: {ApplicationID} " +
                $"PlayerStatus: {PlayerStatus} " +
                $"ConnectionClass: {ConnectionClass} " +
                $"Stats: {System.BitConverter.ToString(Stats)}";
        }
    }
}
