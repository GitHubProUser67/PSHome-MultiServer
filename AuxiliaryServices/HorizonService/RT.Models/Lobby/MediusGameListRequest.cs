using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GameList)]
    public class MediusGameListRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.GameList;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// ApplicationID
        /// </summary>
        public int ApplicatinID;
        /// <summary>
        /// PageID
        /// </summary>
        public ushort PageID;
        /// <summary>
        /// Page Size
        /// </summary>
        public ushort PageSize;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            ApplicatinID = reader.ReadInt32();
            PageID = reader.ReadUInt16();
            PageSize = reader.ReadUInt16();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey);
            writer.Write(2);
            writer.Write(ApplicatinID);
            writer.Write(PageID);
            writer.Write(PageSize);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"Sessionkey: {SessionKey} " +
                $"ApplicationID: {ApplicatinID} " +
                $"PageID: {PageID} " +
                $"PageSize: {PageSize}";
        }
    }
}
