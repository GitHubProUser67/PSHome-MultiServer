using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MatchFindGameRequest)]
    public class MediusMatchFindGameRequest : BaseLobbyExtMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MatchFindGameRequest;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Superset ID
        /// </summary>
        public uint SupersetID;
        /// <summary>
        /// MediusGameWorldID
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// Game Password
        /// </summary>
        public string GamePassword; // GAMEPASSWORD_MAXLEN
        /// <summary>
        /// PlayerJoinType
        /// </summary>
        public MediusJoinType PlayerJoinType;
        /// <summary>
        /// Minimum Players allowed in match
        /// </summary>
        public uint MinPlayers;
        /// <summary>
        /// Maximum Players allowed in match
        /// </summary>
        public uint MaxPlayers;
        /// <summary>
        /// GameHostTypeBitField
        /// </summary>
        public int GameHostTypeBitField;
        /// <summary>
        /// Game Specific Match Options
        /// </summary>
        public MediusMatchOptions MatchOptions;
        /// <summary>
        /// Session Key
        /// </summary>
        public string ServerSessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Game Specific Request Data
        /// </summary>
        public string RequestData;
        /// <summary>
        /// GroupMemberListSize
        /// </summary>
        public uint GroupMemberListSize;
        /// <summary>
        /// ApplicationDataSize
        /// </summary>
        public uint ApplicationDataSize;
        /// <summary>
        /// GroupMemberAccountIDList
        /// </summary>
        public char[] GroupMemberAccountIDList;
        /// <summary>
        /// ApplicationData
        /// </summary>
        public char[] ApplicationData;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            //reader.ReadBytes(2);
            
            //
            SupersetID = reader.ReadUInt32();
            MediusWorldID = reader.ReadInt32();
            PlayerJoinType = reader.Read<MediusJoinType>();
            MinPlayers = reader.ReadUInt32();
            MaxPlayers = reader.ReadUInt32();
            GameHostTypeBitField = reader.ReadInt32();
            MatchOptions = reader.Read<MediusMatchOptions>();
            ServerSessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            RequestData = reader.ReadString(Constants.REQUESTDATA_MAXLEN);
            //reader.ReadBytes(3);

            //
            GroupMemberListSize = reader.ReadUInt32();
            ApplicationDataSize = reader.ReadUInt32();
            GroupMemberAccountIDList = reader.ReadChars((int)GroupMemberListSize);
            ApplicationData = reader.ReadChars((int)ApplicationDataSize);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            //writer.Write(new byte[2]);

            //
            writer.Write(SupersetID);
            writer.Write(MediusWorldID);
            writer.Write(PlayerJoinType);
            writer.Write(MinPlayers);
            writer.Write(MaxPlayers);
            writer.Write(GameHostTypeBitField);
            writer.Write(MatchOptions);
            writer.Write(ServerSessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(RequestData, Constants.REQUESTDATA_MAXLEN);
            //writer.Write(new byte[3]);

            //
            writer.Write(GroupMemberListSize);
            writer.Write(ApplicationDataSize);
            writer.Write(GroupMemberAccountIDList);
            writer.Write(ApplicationData);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"SupersetID: {SupersetID} " +
                $"GameWorldID: {MediusWorldID} " +
                $"PlayerJoinType: {PlayerJoinType} " +
                $"MinPlayers: {MinPlayers} " +
                $"MaxPlayers: {MaxPlayers} " +
                $"GameHostTypeBitField: {GameHostTypeBitField} " +
                $"MatchOptions: {MatchOptions} " +
                $"ServerSessionKey: {ServerSessionKey} " +
                $"RequestData: {RequestData} " +
                $"GroupMemberListSize: {GroupMemberListSize} " +
                $"ApplicationDataSize: {ApplicationDataSize} " +
                $"GroupMemberAccountIDList: {GroupMemberAccountIDList} " +
                $"ApplicationData: {ApplicationData} ";
        }
    }
}
