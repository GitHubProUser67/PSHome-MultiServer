using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.CreateChannel)]
    public class MediusCreateChannelRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.CreateChannel;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int ApplicationID;
        public int MaxPlayers;
        public string LobbyName; // LOBBYNAME_MAXLEN
        public string LobbyPassword; // LOBBYPASSWORD_MAXLEN
        public uint GenericField1;
        public uint GenericField2;
        public uint GenericField3;
        public uint GenericField4;
        public MediusWorldGenericFieldLevelType GenericFieldLevel;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);

            ApplicationID = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            LobbyName = reader.ReadString(Constants.LOBBYNAME_MAXLEN);
            LobbyPassword = reader.ReadString(Constants.LOBBYPASSWORD_MAXLEN);
            GenericField1 = reader.ReadUInt32();
            GenericField2 = reader.ReadUInt32();
            GenericField3 = reader.ReadUInt32();
            GenericField4 = reader.ReadUInt32();
            GenericFieldLevel = reader.Read<MediusWorldGenericFieldLevelType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);

            writer.Write(ApplicationID);
            writer.Write(MaxPlayers);
            writer.Write(LobbyName, Constants.LOBBYNAME_MAXLEN);
            writer.Write(LobbyPassword, Constants.LOBBYPASSWORD_MAXLEN);
            writer.Write(GenericField1);
            writer.Write(GenericField2);
            writer.Write(GenericField3);
            writer.Write(GenericField4);
            writer.Write(GenericFieldLevel);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"ApplicationID: {ApplicationID} " +
                $"MaxPlayers: {MaxPlayers} " +
                $"LobbyName: {LobbyName} " +
                $"LobbyPassword: {LobbyPassword} " +
                $"GenericField1: {GenericField1:X8} " +
                $"GenericField2: {GenericField2:X8} " +
                $"GenericField3: {GenericField3:X8} " +
                $"GenericField4: {GenericField4:X8} " +
                $"GenericFieldLevel: {GenericFieldLevel}";
        }
    }
}
