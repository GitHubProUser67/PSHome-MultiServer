using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request the structure used when calling MGCLSetServerAttributes() to set the <br></br>
    /// game server attributes based on the bit field in MGCL_SERVER_ATTRIBUTES. <br></br>
    /// This structure determines if this is a rebroadcast or a spectator type of <br></br>
    /// server. Usually, peer-to-peer clients do not need to make this call.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerSetAttributesRequest)]
    public class MediusServerSetAttributesRequest : BaseMGCLMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerSetAttributesRequest;

        /// <summary>
        /// Message ID used for asynchronous request processing.
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// MGCL_SERVER_ATTRIBUTES bit-wise OR'ed flag.
        /// </summary>
        public MGCL_SERVER_ATTRIBUTES Attributes;
        /// <summary>
        /// IP address and port for the listen server.
        /// </summary>
        public NetAddress ListenServerAddress;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            Attributes = reader.Read<MGCL_SERVER_ATTRIBUTES>();
            ListenServerAddress = reader.Read<NetAddress>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(Attributes);
            writer.Write(ListenServerAddress);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Attributes: {Attributes} " +
                $"ListenServerAddress: {ListenServerAddress}";
        }
    }
}
