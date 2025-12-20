using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// CreateChannelRequest (Pre 1.50)
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.CreateChannelRequest0)]
    public class MediusCreateChannelRequest0 : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.CreateChannelRequest0;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int ApplicationID;
        public int MinPlayers;
        public int MaxPlayers;
        public int GameLevel;
        public string GamePassword; // GAMEPASSWORD_MAXLEN
        public int PlayerSkillLevel;
        public int RulesSet;
        public uint GenericField1;
        public uint GenericField2;
        public uint GenericField3;
        public MGCL_GAME_HOST_TYPE GameHostType;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            ApplicationID = reader.ReadInt32();
            MinPlayers = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            GameLevel = reader.ReadInt32();
            GamePassword = reader.ReadString(Constants.GAMEPASSWORD_MAXLEN);
            PlayerSkillLevel = reader.ReadInt32();
            RulesSet = reader.ReadInt32();
            GenericField1 = reader.ReadUInt32();
            GenericField2 = reader.ReadUInt32();
            GenericField3 = reader.ReadUInt32();
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(ApplicationID);
            writer.Write(MinPlayers);
            writer.Write(MaxPlayers);
            writer.Write(GameLevel);
            writer.Write(GamePassword);
            writer.Write(PlayerSkillLevel);
            writer.Write(RulesSet);
            writer.Write(GenericField1);
            writer.Write(GenericField2);
            writer.Write(GenericField3);
            writer.Write(GameHostType);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"ApplicationID: {ApplicationID} " +
                $"MinPlayers: {MinPlayers} " +
                $"MaxPlayers: {MaxPlayers} " +
                $"GameLevel: {GameLevel} " +
                $"GamePassword: {GamePassword} " +
                $"PlayerSkillLevel: {PlayerSkillLevel} " +
                $"Ruleset: {RulesSet} " +
                $"GenericField1: {GenericField1:X8} " +
                $"GenericField2: {GenericField2:X8} " +
                $"GenericField3: {GenericField3:X8} " +
                $"GameHostType: {GameHostType}";
        }
    }
}
