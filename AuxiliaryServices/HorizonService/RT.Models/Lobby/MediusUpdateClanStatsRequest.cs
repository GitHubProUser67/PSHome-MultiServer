using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.UpdateClanStats)]
    public class MediusUpdateClanStatsRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.UpdateClanStats;

        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int ClanID;
        public byte[] Stats; // CLANSTATS_MAXLEN

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);

            //
            ClanID = reader.ReadInt32();
            Stats = reader.ReadBytes(Constants.CLANSTATS_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);

            //
            writer.Write(ClanID);
            writer.Write(Stats, Constants.CLANSTATS_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"ClanID: {ClanID} " +
                $"Stats: {string.Join("-", Stats)}";
        }
    }
}
