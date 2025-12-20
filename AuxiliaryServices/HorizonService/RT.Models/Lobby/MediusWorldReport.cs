using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.WorldReport)]
    public class MediusWorldReport : BaseLobbyExtMessage
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.WorldReport;

        public int MediusWorldID;
        public int PlayerCount;
        public string GameName; // GAMENAME_MAXLEN
        public byte[] GameStats = new byte[Constants.GAMESTATS_MAXLEN]; // GAMESTATS_MAXLEN
        public int MinPlayers;
        public int MaxPlayers;
        public int GameLevel;
        public int PlayerSkillLevel;
        public int RulesSet;
        public int GenericField1;
        public int GenericField2;
        public int GenericField3;
        public int GenericField4;
        public int GenericField5;
        public int GenericField6;
        public int GenericField7;
        public int GenericField8;
        public MediusWorldStatus WorldStatus;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MediusWorldID = reader.ReadInt32();
            PlayerCount = reader.ReadInt32();
            GameName = reader.ReadString(Constants.GAMENAME_MAXLEN);
            GameStats = reader.ReadBytes(Constants.GAMESTATS_MAXLEN);
            MinPlayers = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            GameLevel = reader.ReadInt32();
            PlayerSkillLevel = reader.ReadInt32();
            RulesSet = reader.ReadInt32();
            GenericField1 = reader.ReadInt32();
            GenericField2 = reader.ReadInt32();
            GenericField3 = reader.ReadInt32();
            GenericField4 = reader.ReadInt32();
            GenericField5 = reader.ReadInt32();
            GenericField6 = reader.ReadInt32();
            GenericField7 = reader.ReadInt32();
            GenericField8 = reader.ReadInt32();
            WorldStatus = reader.Read<MediusWorldStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MediusWorldID);
            writer.Write(PlayerCount);
            writer.Write(GameName, Constants.GAMENAME_MAXLEN);
            writer.Write(GameStats, Constants.GAMESTATS_MAXLEN);
            writer.Write(MinPlayers);
            writer.Write(MaxPlayers);
            writer.Write(GameLevel);
            writer.Write(PlayerSkillLevel);
            writer.Write(RulesSet);
            writer.Write(GenericField1);
            writer.Write(GenericField2);
            writer.Write(GenericField3);
            writer.Write(GenericField4);
            writer.Write(GenericField5);
            writer.Write(GenericField6);
            writer.Write(GenericField7);
            writer.Write(GenericField8);
            writer.Write(WorldStatus);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MediusWorldID: {MediusWorldID} " +
                $"PlayerCount:{PlayerCount} " +
                $"GameName: {GameName} " +
                $"GameStats: {System.BitConverter.ToString(GameStats)} " +
                $"MinPlayers: {MinPlayers} " +
                $"MaxPlayers: {MaxPlayers} " +
                $"GameLevel: {GameLevel} " +
                $"PlayerSkillLevel: {PlayerSkillLevel} " +
                $"RulesSet: {RulesSet} " +
                $"GenericField1: {GenericField1:X8} " +
                $"GenericField2: {GenericField2:X8} " +
                $"GenericField3: {GenericField3:X8} " +
                $"GenericField4: {GenericField4:X8} " +
                $"GenericField5: {GenericField5:X8} " +
                $"GenericField6: {GenericField6:X8} " +
                $"GenericField7: {GenericField7:X8} " +
                $"GenericField8: {GenericField8:X8} " +
                $"WorldStatus: {WorldStatus}";
        }
    }
}
