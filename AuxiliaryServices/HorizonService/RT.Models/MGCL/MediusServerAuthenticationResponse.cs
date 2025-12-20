using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// he Confirmation value maps to MGCL_ERROR_CODE.<Br></Br>
    /// If it is MGCL_SUCCESS, then the ConnectInfo points to the proxy server to <br></br>
    /// which this host can connect in order to interface with the Medius universe.<br></br>
    /// Save the connect information in a variable, and use it with MGCL_Connect().
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerAuthenticationResponse)]
    public class MediusServerAuthenticationResponse : BaseMGCLMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerAuthenticationResponse;

        /// <summary>
        /// Message ID used for asynchronous request processing.
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// MGCL_SUCCESS or an error.
        /// </summary>
        public MGCL_ERROR_CODE Confirmation;
        /// <summary>
        /// Address of the proxy server to connect to.
        /// </summary>
        public NetConnectionInfo ConnectInfo;

        public bool IsSuccess => Confirmation >= 0;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
            reader.ReadBytes(2);
            ConnectInfo = reader.Read<NetConnectionInfo>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(Confirmation);
            writer.Write(new byte[2]);
            writer.Write(ConnectInfo);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"Confirmation: {Confirmation} " +
                $"ConnectInfo: {ConnectInfo}";
        }
    }
}
