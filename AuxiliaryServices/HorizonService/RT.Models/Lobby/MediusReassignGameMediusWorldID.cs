using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Indication that the "MediusWorldID" of a game has been changed. <br></br>
    /// Use the new value in all subsequent requests/reports
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ReassignGameMediusWorldID)]
    public class MediusReassignGameMediusWorldID : BaseLobbyMessage
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.ReassignGameMediusWorldID;

        /// <summary>
        /// Old Medius game world ID
        /// </summary>
        public int OldMediusWorldID;

        /// <summary>
        /// New Medius game world ID
        /// </summary>
        public int NewMediusWorldID;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            OldMediusWorldID = reader.ReadInt32();
            NewMediusWorldID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(OldMediusWorldID);
            writer.Write(NewMediusWorldID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"OldMediusWorldID: {OldMediusWorldID} " +
                $"NewMediusWorldID: {NewMediusWorldID} ";
        }
    }
}
