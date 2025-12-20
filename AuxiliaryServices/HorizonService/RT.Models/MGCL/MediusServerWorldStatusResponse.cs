using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerWorldStatusResponse)]
    public class MediusServerWorldStatusResponse : BaseMGCLMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerWorldStatusResponse;

        public MessageId MessageID { get; set; }
        public int ApplicationID;
        public int MaxClients;
        public int ActiveClients;
        public MGCL_ERROR_CODE Confirmation;

        public bool IsSuccess => Confirmation >= 0;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            ApplicationID = reader.ReadInt32();
            MaxClients = reader.ReadInt32();
            ActiveClients = reader.ReadInt32();
            Confirmation = reader.Read<MGCL_ERROR_CODE>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(ApplicationID);
            writer.Write(MaxClients);
            writer.Write(ActiveClients);
            writer.Write(Confirmation);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ApplicationID: {ApplicationID} " +
                $"MaxClients: {MaxClients} " +
                $"ActiveClients: {ActiveClients} " +
                $"Confirmation: {Confirmation}";
        }
    }
}
