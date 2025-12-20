using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerSessionBeginRequest)]
    public class MediusServerSessionBeginRequest : BaseMGCLMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerSessionBeginRequest;

        public MessageId MessageID { get; set; }
        public int LocationID;
        public int ApplicationID;
        public MGCL_GAME_HOST_TYPE ServerType;
        public string ServerVersion; // MGCL_SERVERVERSION_MAXLEN
        /// <summary>
        /// Not used in version 2.10
        /// </summary>
        public int Port;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            LocationID = reader.ReadInt32();
            ApplicationID = reader.ReadInt32();
            ServerType = reader.Read<MGCL_GAME_HOST_TYPE>();
            ServerVersion = reader.ReadString(Constants.MGCL_SERVERVERSION_MAXLEN);
            Port = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(LocationID);
            writer.Write(ApplicationID);
            writer.Write(ServerType);
            writer.Write(ServerVersion, Constants.MGCL_SERVERVERSION_MAXLEN);
            writer.Write(Port);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"LocationID: {LocationID} " +
                $"ApplicationID: {ApplicationID} " +
                $"ServerType: {ServerType} " +
                $"ServerVersion: {ServerVersion} " +
                $"Port: {Port}(ignored)";
        }
    }
}
