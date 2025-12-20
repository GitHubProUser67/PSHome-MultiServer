using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GameInfoResponse)]
    public class MediusGameInfoResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GameInfoResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int ApplicationID;
        public int MinPlayers;
        public int MaxPlayers;
        public int GameLevel;
        public int PlayerSkillLevel;
        public int PlayerCount;
        public byte[] GameStats = new byte[Constants.GAMESTATS_MAXLEN];
        public string GameName; // GAMENAME_MAXLEN
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
        public MGCL_GAME_HOST_TYPE GameHostType;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            ApplicationID = reader.ReadInt32();
            MinPlayers = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            GameLevel = reader.ReadInt32();
            PlayerSkillLevel = reader.ReadInt32();
            PlayerCount = reader.ReadInt32();
            GameStats = reader.ReadBytes(Constants.GAMESTATS_MAXLEN);
            GameName = reader.ReadString(Constants.GAMENAME_MAXLEN);
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
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
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
            writer.Write(ApplicationID);
            writer.Write(MinPlayers);
            writer.Write(MaxPlayers);
            writer.Write(GameLevel);
            writer.Write(PlayerSkillLevel);
            writer.Write(PlayerCount);
            writer.Write(GameStats, Constants.GAMESTATS_MAXLEN);
            writer.Write(GameName, Constants.GAMENAME_MAXLEN);
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
            writer.Write(GameHostType);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
             $"StatusCode:{StatusCode} " +
$"ApplicationID:{ApplicationID} " +
$"MinPlayers:{MinPlayers} " +
$"MaxPlayers:{MaxPlayers} " +
$"GameLevel:{GameLevel} " +
$"PlayerSkillLevel:{PlayerSkillLevel} " +
$"PlayerCount:{PlayerCount} " +
$"GameStats:{GameStats} " +
$"GameName:{GameName} " +
$"RulesSet:{RulesSet} " +
$"GenericField1:{GenericField1:X8} " +
$"GenericField2:{GenericField2:X8} " +
$"GenericField3:{GenericField3:X8} " +
$"GenericField4:{GenericField4:X8} " +
$"GenericField5:{GenericField5:X8} " +
$"GenericField6:{GenericField6:X8} " +
$"GenericField7:{GenericField7:X8} " +
$"GenericField8:{GenericField8:X8} " +
$"WorldStatus:{WorldStatus} " +
$"GameHostType:{GameHostType}";
        }
    }
}
