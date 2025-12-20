using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// A Medius server is telling this host to end a game. It is invoked through <br></br>
    /// the callback defined at MGCL initialization time. Peer-to-peer clients <br></br>
    /// usually do not need to consider this request.<br></br>
    /// The BrutalFlag is either 0 or 1. Under normal circumstances, <Br></Br>
    /// the game should end when the client count for the game world reaches <br></br>
    /// zero. If the brutal flag is True, then the world should be destroyed <Br></Br>
    /// immediately, and all of the clients forcefully disconnected.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerEndGameRequest)]
    public class MediusServerEndGameRequest : BaseMGCLMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerEndGameRequest;

        /// <summary>
        /// Message ID used for asynchronous request processing.
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// World ID of the game to kill.
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// Boolean, to either Kill now, or allow the game to finish and then destroy the game world.
        /// </summary>
        public bool BrutalFlag;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            MediusWorldID = reader.ReadInt32();
            BrutalFlag = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(MediusWorldID);
            writer.Write(BrutalFlag);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"BrutalFlag: {BrutalFlag}";
        }
    }
}
