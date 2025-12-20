using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.CreateGame1)]
    public class MediusCreateGameRequest1 : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.CreateGame1;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int ApplicationID;
        public int MinPlayers;
        public int MaxPlayers;
        public int GameLevel;
        public string GameName; // GAMENAME_MAXLEN
        public string GamePassword; // GAMEPASSWORD_MAXLEN
        public string SpectatorPassword; // GAMEPASSWORD_MAXLEN
        public int PlayerSkillLevel;
        public int RulesSet;
        public int GenericField1;
        public int GenericField2;
        public int GenericField3;
        public MGCL_GAME_HOST_TYPE GameHostType;
        public MediusWorldAttributesType WorldAttributesType;

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
            GameName = reader.ReadString(Constants.GAMENAME_MAXLEN);
            GamePassword = reader.ReadString(Constants.GAMEPASSWORD_MAXLEN);
            SpectatorPassword = reader.ReadString(Constants.GAMEPASSWORD_MAXLEN);
            PlayerSkillLevel = reader.ReadInt32();
            RulesSet = reader.ReadInt32();
            GenericField1 = reader.ReadInt32();
            GenericField2 = reader.ReadInt32();
            GenericField3 = reader.ReadInt32();
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
            //WorldAttributesType = reader.Read<MediusWorldAttributesType>();
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
            writer.Write(GameName, Constants.GAMENAME_MAXLEN);
            writer.Write(GamePassword, Constants.GAMEPASSWORD_MAXLEN);
            writer.Write(SpectatorPassword, Constants.GAMEPASSWORD_MAXLEN);
            writer.Write(PlayerSkillLevel);
            writer.Write(RulesSet);
            writer.Write(GenericField1);
            writer.Write(GenericField2);
            writer.Write(GenericField3);
            writer.Write(GameHostType);
            //writer.Write(WorldAttributesType);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"SessionKey:{SessionKey} " +
                $"ApplicationID:{ApplicationID} " +
                $"MinPlayers:{MinPlayers} " +
                $"MaxPlayers:{MaxPlayers} " +
                $"GameLevel:{GameLevel} " +
                $"GameName:{GameName} " +
                $"GamePassword:{GamePassword} " +
                $"SpectatorPassword:{SpectatorPassword} " +
                $"PlayerSkillLevel:{PlayerSkillLevel} " +
                $"RulesSet:{RulesSet} " +
                $"GenericField1:{GenericField1:X8} " +
                $"GenericField2:{GenericField2:X8} " +
                $"GenericField3:{GenericField3:X8} " +
                $"GameHostType:{GameHostType} " +
                $"WorldAttributesType: {WorldAttributesType}";
        }
    }
}
