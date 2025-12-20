using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.PartyCreateRequest)]
    public class MediusPartyCreateRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.PartyCreateRequest;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Application ID of this party to create on
        /// </summary>
        public int ApplicationID;
        /// <summary>
        /// Minimum amount of players allowed in a party
        /// </summary>
        public int MinPlayers;
        /// <summary>
        /// Maximum amount of players allowed in a party
        /// </summary>
        public int MaxPlayers;
        /// <summary>
        /// Party Name set in the request
        /// </summary>
        public string PartyName; // PARTYNAME_MAXLEN
        /// <summary>
        /// Party Password set in the request
        /// </summary>
        public string PartyPassword; // PARTYPASSWORD_MAXLEN
        public int GenericField1;
        public int GenericField2;
        public int GenericField3;
        public int GenericField4;
        public int GenericField5;
        public int GenericField6;
        public int GenericField7;
        public int GenericField8;
        /// <summary>
        /// PartyHostType of this party
        /// </summary>
        public MGCL_GAME_HOST_TYPE PartyHostType;
        public string ServerSessionKey; //SESSIONKEY_MAXLEN


        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);

            //
            ApplicationID = reader.ReadInt32();
            MinPlayers = reader.ReadInt32();
            MaxPlayers = reader.ReadInt32();
            PartyName = reader.ReadString(Constants.PARTYNAME_MAXLEN);
            PartyPassword = reader.ReadString(Constants.PARTYPASSWORD_MAXLEN);
            GenericField1 = reader.ReadInt32();
            GenericField2 = reader.ReadInt32();
            GenericField3 = reader.ReadInt32();
            GenericField4 = reader.ReadInt32();
            GenericField5 = reader.ReadInt32();
            GenericField6 = reader.ReadInt32();
            GenericField7 = reader.ReadInt32();
            GenericField8 = reader.ReadInt32();
            PartyHostType = reader.Read<MGCL_GAME_HOST_TYPE>();

            ServerSessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            //reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);

            //
            writer.Write(ApplicationID);
            writer.Write(MinPlayers);
            writer.Write(MaxPlayers);
            writer.Write(PartyName, Constants.PARTYNAME_MAXLEN);
            writer.Write(PartyPassword, Constants.PARTYPASSWORD_MAXLEN);
            writer.Write(GenericField1);
            writer.Write(GenericField2);
            writer.Write(GenericField3);
            writer.Write(GenericField4);
            writer.Write(GenericField5);
            writer.Write(GenericField6);
            writer.Write(GenericField7);
            writer.Write(GenericField8);
            writer.Write(PartyHostType);
            writer.Write(ServerSessionKey, Constants.SESSIONKEY_MAXLEN);
            //writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"ApplicationID: {ApplicationID} " +
                $"MinPlayers: {MinPlayers} " +
                $"MaxPlayers: {MaxPlayers} " +
                $"PartyName: {PartyName} " +
                $"PartyPassword: {PartyPassword} " +
                $"GenericField1: {GenericField1} " +
                $"GenericField2: {GenericField2} " +
                $"GenericField3: {GenericField3} " +
                $"GenericField4: {GenericField4} " +
                $"GenericField5: {GenericField5} " +
                $"GenericField6: {GenericField6} " +
                $"GenericField7: {GenericField7} " +
                $"GenericField8: {GenericField8} " +
                $"PartyHostType: {PartyHostType} " +
                $"ServerSessionKey: {ServerSessionKey} ";
        }
    }
}
