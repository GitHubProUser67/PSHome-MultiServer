using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.BanPlayer)]
    public class MediusBanPlayerRequest : BaseLobbyMessage
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.BanPlayer;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// BanAccountID
        /// </summary>
        public int BanAccountID;
        /// <summary>
        /// BanMinutes
        /// </summary>
        public int BanMinutes;
        /// <summary>
        /// MediusWorldID
        /// </summary>
        public int MediusWorldID;
        public MediusApplicationType AppType;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(2);

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            BanAccountID = reader.ReadInt32();
            BanMinutes = reader.ReadInt32();
            MediusWorldID = reader.ReadInt32();
            AppType = reader.Read<MediusApplicationType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[2]);

            writer.Write(SessionKey);
            writer.Write(BanAccountID);
            writer.Write(BanMinutes);
            writer.Write(MediusWorldID);
            writer.Write(AppType);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"AccountID of Player to ban {BanAccountID} " +
                $"Minutes to ban player {BanMinutes} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"AppType: {AppType}";
        }
    }
}
