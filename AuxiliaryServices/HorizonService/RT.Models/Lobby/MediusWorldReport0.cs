using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.WorldReport0)]
    public class MediusWorldReport0 : BaseLobbyMessage
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.WorldReport0;

        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey;
        /// <summary>
        /// MediusWorldID
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// PlayerCount
        /// </summary>
        public int PlayerCount;
        /// <summary>
        /// GameName
        /// </summary>
        public string GameName; // GAMENAME_MAXLEN
        /// <summary>
        /// GameStats
        /// </summary>
        public byte[] GameStats = new byte[Constants.GAMESTATS_MAXLEN]; // GAMESTATS_MAXLEN
        /// <summary>
        /// Minimum Players
        /// </summary>
        public int MinPlayers;
        /// <summary>
        /// Maximum Players
        /// </summary>
        public int MaxPlayers;
        /// <summary>
        /// Game Level
        /// </summary>
        public int GameLevel;
        /// <summary>
        /// Player Skill Level
        /// </summary>
        public int PlayerSkillLevel;
        /// <summary>
        /// RulesSet
        /// </summary>
        public int RulesSet;
        /// <summary>
        /// GenericField1
        /// </summary>
        public int GenericField1;
        /// <summary>
        /// GenericField2
        /// </summary>
        public int GenericField2;
        /// <summary>
        /// GenericField3
        /// </summary>
        public int GenericField3;
        /// <summary>
        /// WorldStatus
        /// </summary>
        public MediusWorldStatus WorldStatus;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);

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
            WorldStatus = reader.Read<MediusWorldStatus>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(SessionKey);
            writer.Write(new byte[3]);

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
            writer.Write(WorldStatus);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"SessionKey: {SessionKey} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"PlayerCount: {PlayerCount} " +
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
                $"WorldStatus: {WorldStatus}";
        }
    }
}
