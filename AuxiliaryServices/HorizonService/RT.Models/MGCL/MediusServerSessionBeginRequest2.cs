using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerSessionBeginRequest2)]
    public class MediusServerSessionBeginRequest2 : BaseMGCLMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerSessionBeginRequest2;

        public MessageId MessageID { get; set; }
        /// <summary>
        /// LocaltionID of Client
        /// </summary>
        public int LocationID;
        /// <summary>
        /// Client ApplicationID
        /// </summary>
        public int ApplicationID;
        /// <summary>
        /// GameHostType connecting as.
        /// </summary>
        public MGCL_GAME_HOST_TYPE ServerType;
        /// <summary>
        /// Major version for the Medius Client
        /// </summary>
        public int ClientVersionMajor;
        /// <summary>
        /// Minor version for the Medius Client
        /// </summary>
        public int ClientVersionMinor;
        /// <summary>
        /// Special Patch version for the Medius Client
        /// </summary>
        public int ClientVersionSpecialPatch;
        /// <summary>
        /// Build version for the Medius Client
        /// </summary>
        public int ClientVersionBuild;
        /// <summary>
        /// Not used in version 2.10
        /// </summary>
        public int Port;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            LocationID = reader.ReadInt32();
            ApplicationID = reader.ReadInt32();
            ServerType = reader.Read<MGCL_GAME_HOST_TYPE>();
            ClientVersionMajor = reader.ReadInt32();
            ClientVersionMinor = reader.ReadInt32();
            ClientVersionSpecialPatch = reader.ReadInt32();
            ClientVersionBuild = reader.ReadInt32();
            Port = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(LocationID);
            writer.Write(ApplicationID);
            writer.Write(ServerType);
            writer.Write(ClientVersionMajor);
            writer.Write(ClientVersionMinor);
            writer.Write(ClientVersionSpecialPatch);
            writer.Write(ClientVersionBuild);
            writer.Write(Port);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"LocationID: {LocationID} " +
                $"ApplicationID: {ApplicationID} " +
                $"ServerType: {ServerType} " +
                $"ClientVersionMajor: {ClientVersionMajor} " +
                $"ClientVersionMinor: {ClientVersionMinor} " +
                $"ClientVersionSpecialPatch: {ClientVersionSpecialPatch} " +
                $"ClientVersionBuild: {ClientVersionBuild} " +
                $"Port: {Port}(ignored)";
        }
    }
}
