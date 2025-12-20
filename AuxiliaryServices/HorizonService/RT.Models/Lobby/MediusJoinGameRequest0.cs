using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// MediusJoinGameRequest0 (Pre 1.50)
    /// </summary>
	[MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.JoinGameRequest0)]
    public class MediusJoinGameRequest0 : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.JoinGameRequest0;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int MediusWorldID;
        public string GamePassword; // GAMEPASSWORD_MAXLEN
        public MGCL_GAME_HOST_TYPE GameHostType;
        public NetAddressList AddressList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            MediusWorldID = reader.ReadInt32();
            GamePassword = reader.ReadString(Constants.GAMEPASSWORD_MAXLEN);
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
            AddressList = reader.Read<NetAddressList>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(MediusWorldID);
            writer.Write(GamePassword, Constants.GAMEPASSWORD_MAXLEN);
            writer.Write(GameHostType);
            writer.Write(AddressList);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"GamePassword: {GamePassword} " +
                $"GameHostType: {GameHostType} " +
                $"AddressList: {AddressList}";
        }
    }
}
