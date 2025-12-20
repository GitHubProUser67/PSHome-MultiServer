using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.PartyPlayerReport)]
    public class MediusPartyPlayerReport : BaseLobbyExtMessage
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.PartyPlayerReport;

        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Medius World ID of the party
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// Party ID
        /// </summary>
        public int PartyIndex;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);

            // 
            MediusWorldID = reader.ReadInt32();
            PartyIndex = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);

            // 
            writer.Write(MediusWorldID);
            writer.Write(PartyIndex);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"SessionKey: {SessionKey} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"PartyIndex: {PartyIndex}";
        }
    }
}
