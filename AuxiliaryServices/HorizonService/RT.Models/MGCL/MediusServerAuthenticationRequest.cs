using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerAuthenticationRequest)]
    public class MediusServerAuthenticationRequest : BaseMGCLMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerAuthenticationRequest;

        /// <summary>
        /// Message ID used for asynchronous request processing.
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Trust level for this game server.
        /// </summary>
        public MGCL_TRUST_LEVEL TrustLevel;
        /// <summary>
        /// Server address or port for standalone GS;<Br></Br>
        /// it is only populated internally by MGCL.
        /// </summary>
        public NetAddressList AddressList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            TrustLevel = reader.Read<MGCL_TRUST_LEVEL>();
            AddressList = reader.Read<NetAddressList>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(TrustLevel);
            writer.Write(AddressList);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"TrustLevel: {TrustLevel} " +
                $"AddressList: {AddressList}";
        }
    }
}
