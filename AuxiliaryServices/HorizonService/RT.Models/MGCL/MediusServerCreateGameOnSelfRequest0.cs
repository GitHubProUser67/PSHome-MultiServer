using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerCreateGameOnSelfRequest0)]
    public class MediusServerCreateGameOnSelfRequest0 : BaseMGCLMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerCreateGameOnSelfRequest0;

        public MessageId MessageID { get; set; }
        public string GameName; // MGCL_GAMENAME_MAXLEN
        public byte[] GameStats = new byte[Constants.MGCL_GAMESTATS_MAXLEN];
        public string GamePassword; // MGCL_GAMEPASSWORD_MAXLEN
        public int ApplicationID;
        public int MaxPlayers;
        public int MinPlayers;
        public int GameLevel;
        public int PlayerSkillLevel;
        public int RulesSet;
        public int GenericField1;
        public int GenericField2;
        public int GenericField3;
        public MGCL_GAME_HOST_TYPE GameHostType;
        public NetAddressList AddressList;
        public int WorldID;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            if (reader.MediusVersion == 113)
                GameName = reader.ReadString(Constants.MGCL_GAMENAME_MAXLEN1);
            else
                GameName = reader.ReadString(Constants.MGCL_GAMENAME_MAXLEN);
            GameStats = reader.ReadBytes(Constants.MGCL_GAMESTATS_MAXLEN);

            if (reader.MediusVersion == 113)
                GamePassword = reader.ReadString(Constants.MGCL_GAMEPASSWORD_MAXLEN1);
            else
                GamePassword = reader.ReadString(Constants.MGCL_GAMEPASSWORD_MAXLEN);
            reader.ReadBytes(3);
            ApplicationID = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            MinPlayers = reader.ReadInt32();
            GameLevel = reader.ReadInt32();
            PlayerSkillLevel = reader.ReadInt32();
            RulesSet = reader.ReadInt32();
            GenericField1 = reader.ReadInt32();
            GenericField2 = reader.ReadInt32();
            GenericField3 = reader.ReadInt32();
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
            AddressList = reader.Read<NetAddressList>();
            WorldID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            if (writer.MediusVersion == 113)
                writer.Write(GameName, Constants.MGCL_GAMENAME_MAXLEN1);
            else
                writer.Write(GameName, Constants.MGCL_GAMENAME_MAXLEN);
            writer.Write(GameStats, Constants.MGCL_GAMESTATS_MAXLEN);

            if (writer.MediusVersion == 113)
                writer.Write(GamePassword, Constants.MGCL_GAMEPASSWORD_MAXLEN1);
            else
                writer.Write(GamePassword, Constants.MGCL_GAMEPASSWORD_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(ApplicationID);
            writer.Write(MaxPlayers);
            writer.Write(MinPlayers);
            writer.Write(GameLevel);
            writer.Write(PlayerSkillLevel);
            writer.Write(RulesSet);
            writer.Write(GenericField1);
            writer.Write(GenericField2);
            writer.Write(GenericField3);
            writer.Write(GameHostType);
            writer.Write(AddressList);
            writer.Write(WorldID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"GameName: {GameName} " +
                $"GameStats: {GameStats} " +
                $"GamePassword: {GamePassword} " +
                $"ApplicationID: {ApplicationID} " +
                $"MaxClients: {MaxPlayers} " +
                $"MinClients: {MinPlayers} " +
                $"GameLevel: {GameLevel} " +
                $"PlayerSkillLevel: {PlayerSkillLevel} " +
                $"RulesSet: {RulesSet} " +
                $"GenericField1: {GenericField1:X8} " +
                $"GenericField2: {GenericField2:X8} " +
                $"GenericField3: {GenericField3:X8} " +
                $"GameHostType: {GameHostType} " +
                $"AddressList: {AddressList} " +
                $"WorldID: {WorldID} ";
        }
    }
}
