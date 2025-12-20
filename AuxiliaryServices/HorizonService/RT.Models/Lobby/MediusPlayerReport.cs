using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Report from every player of a game to the Medius Lobby Server, preferably every 30 seconds
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.PlayerReport)]
    public class MediusPlayerReport : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.PlayerReport;

        public MessageId MessageID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// MediusWorldID
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// Account Stats to update
        /// </summary>
        public byte[] Stats = new byte[Constants.ACCOUNTSTATS_MAXLEN];

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);

            MediusWorldID = reader.ReadInt32();
            Stats = reader.ReadBytes(Constants.ACCOUNTSTATS_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);

            writer.Write(MediusWorldID);
            writer.Write(Stats, Constants.ACCOUNTSTATS_MAXLEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"SessionKey: {SessionKey} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"Stats: {BitConverter.ToString(Stats)}";
        }
    }
}
