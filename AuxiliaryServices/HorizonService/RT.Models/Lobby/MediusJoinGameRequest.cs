using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.JoinGame)]
    public class MediusJoinGameRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.JoinGame;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int MediusWorldID;
        public MediusJoinType JoinType;
        public string GamePassword; // GAMEPASSWORD_MAXLEN
        public MGCL_GAME_HOST_TYPE GameHostType;
        public RSA_KEY pubKey;
        public NetAddressList AddressList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            MediusWorldID = reader.ReadInt32();
            JoinType = reader.Read<MediusJoinType>();
            GamePassword = reader.ReadString(Constants.GAMEPASSWORD_MAXLEN);
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
            pubKey = reader.Read<RSA_KEY>();
            AddressList = reader.Read<NetAddressList>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(MediusWorldID);
            writer.Write(JoinType);
            writer.Write(GamePassword, Constants.GAMEPASSWORD_MAXLEN);
            writer.Write(GameHostType);
            writer.Write(pubKey);
            writer.Write(AddressList);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"JoinType: {JoinType} " +
                $"GamePassword: {GamePassword} " +
                $"GameHostType: {GameHostType} " +
                $"pubKey: {pubKey} " +
                $"AddressList: {AddressList}";
        }
    }
}
