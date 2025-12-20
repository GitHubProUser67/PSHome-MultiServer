using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MatchSetGameStateRequest)]
    public class MediusMatchPartyResponse : BaseLobbyExtMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MatchSetGameStateRequest;

        public bool IsSuccess => StatusCode >= 0;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int PluginSpecificStatusCode;

        //MediusJoinAssignedGame 
        public int GameWorldID;
        public string GamePassword = ""; //GAMEPASSWORD_MAXLEN
        public MGCL_GAME_HOST_TYPE GameHostType;
        public NetAddressList AddressList;
        public int ApplicationDataSizeJAS;
        public string ApplicationDataJAS;
        public int MatchRoster;

        // MatchRosterInfo
        public int NumParties; //RosterSize?
        public int Parties;

        // MatchPartyInfo
        public int NumPlayers; 
        public int Players;

        //MediusMatchTypeHostGame
        public int MatchGameID;
        public int ApplicationDataSizeHG;
        public string ApplicationDataHG;

        // MediusMatchTypeReferral
        /// <summary>
        /// MatchingWorldUID to connect to 
        /// </summary>
        public int MatchingWorldUID;
        /// <summary>
        /// NetConnectionInfo of Medius Matchmaking Server
        /// </summary>
        public NetConnectionInfo ConnectInfo;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            //
            StatusCode = reader.Read<MediusCallbackStatus>();
            PluginSpecificStatusCode = reader.ReadInt32();

            //MediusMatchJoinSpecified
            if (StatusCode == MediusCallbackStatus.MediusJoinAssignedGame)
            {
                GameWorldID = reader.ReadInt32();
                GamePassword = reader.ReadString(Constants.GAMEPASSWORD_MAXLEN);
                GameHostType = reader.Read<MGCL_GAME_HOST_TYPE>();
                AddressList = reader.Read<NetAddressList>();
                ApplicationDataSizeJAS = reader.ReadInt32();
                ApplicationDataJAS = reader.ReadString(ApplicationDataSizeJAS);
                MatchRoster = reader.ReadInt32();

                //MediusMatchRosterInfoMarshal
                NumPlayers = reader.ReadInt32();
                Players = reader.ReadInt32();

                //MediusMatchPartyInfoMarshal
                NumParties = reader.ReadInt32();
                Parties = reader.ReadInt32();
            }


            //MediusMatchTypeHostGame
            if (StatusCode == MediusCallbackStatus.MediusMatchTypeHostGame)
            {
                MatchGameID = reader.ReadInt32();
                ApplicationDataSizeHG = reader.ReadInt32();
                ApplicationDataHG = reader.ReadString(ApplicationDataSizeHG);
            }

            //MediusMatchTypeReferral
            if(StatusCode == MediusCallbackStatus.MediusMatchTypeReferral)
            {
                MatchingWorldUID = reader.ReadInt32();
                ConnectInfo = reader.Read<NetConnectionInfo>();
            }

        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            //
            writer.Write(StatusCode);
            writer.Write(PluginSpecificStatusCode);

            if (StatusCode == MediusCallbackStatus.MediusJoinAssignedGame)
            {
                writer.Write(GameWorldID);
                writer.Write(GamePassword);
                writer.Write(GameHostType);
                writer.Write(AddressList);
                writer.Write(ApplicationDataSizeJAS);
                writer.Write(ApplicationDataJAS, ApplicationDataSizeJAS);
                writer.Write(MatchRoster);

                writer.Write(NumPlayers);
                writer.Write(Players);

                writer.Write(NumParties);
                writer.Write(Parties);
            }

            if (StatusCode == MediusCallbackStatus.MediusMatchTypeHostGame)
            {
                writer.Write(MatchGameID);
                writer.Write(ApplicationDataSizeHG);
                writer.Write(ApplicationDataHG, ApplicationDataSizeHG);
            }

            if (StatusCode == MediusCallbackStatus.MediusMatchTypeReferral)
            {
                writer.Write(MatchingWorldUID);
                writer.Write(ConnectInfo);
            }
        }

        public override string ToString()
        {
            if (StatusCode == MediusCallbackStatus.MediusJoinAssignedGame)
            {
                return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"PluginSpecificStatusCode: {PluginSpecificStatusCode} " +
                $"GameWorldID: {GameWorldID} " +
                $"GamePassword: {GamePassword} " +
                $"GameHostType: {GameHostType} " +
                $"AddressList: {AddressList} " +
                $"ApplicationDataSize: {ApplicationDataSizeJAS} " +
                $"ApplicationData: {ApplicationDataJAS} " +
                $"MatchRoster: {MatchRoster} " +
                $"NumPlayers: {NumPlayers} " +
                $"Players: {Players} " +
                $"NumParties: {NumParties} " +
                $"Parties: {Parties}";
            }

            if (StatusCode == MediusCallbackStatus.MediusMatchTypeHostGame)
            {
                return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"PluginSpecificStatusCode: {PluginSpecificStatusCode} " +
                $"MatchGameID: {MatchGameID} " + 
                $"ApplicationDataSize: {ApplicationDataSizeHG} " +
                $"ApplicationData: {ApplicationDataHG}";
            }

            if (StatusCode == MediusCallbackStatus.MediusMatchTypeReferral)
            {
                return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"PluginSpecificStatusCode: {PluginSpecificStatusCode} " +
                $"MatchingWorldUID: {MatchingWorldUID} " +
                $"ConnectInfo: {ConnectInfo}";
            }

            return base.ToString();
        }
    }
}
