using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request a structure to notify Medius about the connect or disconnnect of a client on this game host.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerConnectNotification)]
    public class MediusServerConnectNotification : BaseMGCLMessage
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerConnectNotification;

        /// <summary>
        /// A connect or disconnect event.
        /// </summary>
        public MGCL_EVENT_TYPE ConnectEventType;
        /// <summary>
        /// Medius game world Unique ID that the player
        /// connected or disconnected from.
        /// </summary>
        public int MediusWorldUID;
        /// <summary>
        /// The player's session key.
        /// </summary>
        public string PlayerSessionKey;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            ConnectEventType = reader.Read<MGCL_EVENT_TYPE>();
            MediusWorldUID = reader.ReadInt32();
            PlayerSessionKey = reader.ReadString(Constants.MGCL_SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(ConnectEventType);
            writer.Write(MediusWorldUID);
            writer.Write(PlayerSessionKey, Constants.MGCL_SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"ConnectEventType: {ConnectEventType} " +
                $"MediusWorldUID: {MediusWorldUID} " +
                $"PlayerSessionKey: {PlayerSessionKey}";
        }
    }
}
