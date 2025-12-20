using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MatchCreateGameRequest)]
    public class MediusMatchCreateGameRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MatchCreateGameRequest;

        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int SupersetID;
        public int ApplicationID;
        public int MinPlayers;
        public int MaxPlayers;
        public int GameLevel;
        public string GameName;
        public string GamePassword;
        public string SpectatorPassword;
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
        public MGCL_GAME_HOST_TYPE GameHostType;
        public MediusWorldAttributesType WorldAttributesType;
        public MediusMatchOptions MatchOptions;
        public string ServerSessionKey; // SESSIONKEY_MAXLEN
        public byte[] RequestData = new byte[Constants.REQUESTDATA_MAXLEN]; // REQUESTDATA_MAXLEN
        public uint GroupMemberListSize;
        public uint ApplicationDataSize;
        public byte[] GroupMemberAccountIDList;
        public string ApplicationData;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            SupersetID = reader.ReadInt32();
            ApplicationID = reader.ReadInt32();
            MinPlayers = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            GameLevel = reader.ReadInt32();
            GameName = reader.ReadString(Constants.GAMENAME_MAXLEN);
            GamePassword = reader.ReadString(Constants.GAMEPASSWORD_MAXLEN);
            SpectatorPassword = reader.ReadString(Constants.SPECTATORPASSWORD_MAXLEN);
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
            GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
            WorldAttributesType = reader.Read<MediusWorldAttributesType>();
            MatchOptions = reader.Read<MediusMatchOptions>();
            ServerSessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            RequestData = reader.ReadBytes(Constants.REQUESTDATA_MAXLEN);
            GroupMemberListSize = reader.ReadUInt32();
            ApplicationDataSize = reader.ReadUInt32();
            GroupMemberAccountIDList = reader.ReadBytes((int)GroupMemberListSize);
            ApplicationData = reader.ReadString((int)GroupMemberListSize);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(SupersetID);
            writer.Write(ApplicationID);
            writer.Write(MinPlayers);
            writer.Write(MaxPlayers);
            writer.Write(GameLevel);
            writer.Write(GameName, Constants.GAMENAME_MAXLEN);
            writer.Write(GamePassword, Constants.GAMEPASSWORD_MAXLEN);
            writer.Write(SpectatorPassword, Constants.SPECTATORPASSWORD_MAXLEN);
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
            writer.Write(GameHostType);
            writer.Write(WorldAttributesType);
            writer.Write(MatchOptions);
            writer.Write(ServerSessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(RequestData);
            writer.Write(GroupMemberListSize);
            writer.Write(ApplicationDataSize);
            writer.Write(GroupMemberAccountIDList);
            writer.Write(ApplicationData, (int)ApplicationDataSize);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"SupersetID: {SupersetID} " +
                $"ApplicationID: {ApplicationID} " +
                $"MinPlayers: {MinPlayers} " +
                $"MaxPlayers: {MaxPlayers} " +
                $"GameLevel: {GameLevel} " +
                $"GameName: {GameName} " +
                $"GamePassword: {GamePassword} " +
                $"SpectatorPassword: {SpectatorPassword} " +
                $"PlayerSkillLevel: {PlayerSkillLevel} " +
                $"RulesSet: {RulesSet} " +
                $"GenericField1: {GenericField1} " +
                $"GenericField2: {GenericField2} " +
                $"GenericField3: {GenericField3} " +
                $"GenericField4: {GenericField4} " +
                $"GenericField5: {GenericField5} " +
                $"GenericField6: {GenericField6} " +
                $"GenericField7: {GenericField7} " +
                $"GenericField8: {GenericField8} " +
                $"GameHostType: {GameHostType} " +
                $"WorldAttributesType: {WorldAttributesType} " +
                $"MatchOptions: {MatchOptions} " +
                $"ServerSessionKey: {ServerSessionKey} " +
                $"RequestData: {RequestData} " +
                $"GroupMemberListSize: {GroupMemberListSize} " +
                $"ApplicationDataSize: {ApplicationDataSize} " +
                $"GroupMemberAccountIDList: {GroupMemberAccountIDList} " +
                $"ApplicationData: {string.Join(string.Empty, ApplicationData)}";
        }
    }
}
