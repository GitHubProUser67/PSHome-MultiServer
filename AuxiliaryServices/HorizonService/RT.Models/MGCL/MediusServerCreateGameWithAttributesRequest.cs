using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerCreateGameWithAttributesRequest)]
    public class MediusServerCreateGameWithAttributesRequest : BaseMGCLMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerCreateGameWithAttributesRequest;

        public MessageId MessageID { get; set; }
        public int ApplicationID;
        public int MaxClients;
        public MediusWorldAttributesType Attributes;
        public int WorldID;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            ApplicationID = reader.ReadInt32();
            MaxClients = reader.ReadInt32();
            Attributes = reader.Read<MediusWorldAttributesType>();
            WorldID = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(ApplicationID);
            writer.Write(MaxClients);
            writer.Write(Attributes);
            writer.Write(WorldID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ApplicationID: {ApplicationID} " +
                $"MaxClients: {MaxClients} " +
                $"Attributes: {Attributes} " +
                $"WorldUID: {WorldID}";
        }
    }
}
