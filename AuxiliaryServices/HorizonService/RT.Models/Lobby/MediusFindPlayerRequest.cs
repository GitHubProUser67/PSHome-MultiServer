using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request to search for a player
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FindPlayer)]
    public class MediusFindPlayerRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FindPlayer;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Type of search (by ID or name)
        /// </summary>
        public MediusPlayerSearchType SearchType;
        /// <summary>
        /// ID of player to find.
        /// </summary>
        public int ID;
        /// <summary>
        /// Name of Player to find
        /// </summary>
        public string Name; // PLAYERNAME_MAXLEN

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
            SearchType = reader.Read<MediusPlayerSearchType>();
            ID = reader.ReadInt32();
            Name = reader.ReadString(Constants.PLAYERNAME_MAXLEN);
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
            writer.Write(SearchType);
            writer.Write(ID);
            writer.Write(Name, Constants.PLAYERNAME_MAXLEN);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"SearchType: {SearchType} " +
                $"ID: {ID} " +
                $"Name: {Name}";
        }
    }
}
