using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models.MGCL
{
    [MediusMessage(NetMessageClass.MessageClassLobbyReport, MediusMGCLMessageIds.ServerCreateGameWithAttributesRequest2)]
    public class MediusServerCreateGameWithAttributesRequest2 : BaseMGCLMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusMGCLMessageIds.ServerCreateGameWithAttributesRequest2;

        public MessageId MessageID { get; set; }
        public int ApplicationID;
        public int MaxClients;
        public MediusWorldAttributesType Attributes;
        public int MediusWorldUID;
        public NetConnectionInfo ConnectInfo;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            ApplicationID = reader.ReadInt32();
            MaxClients = reader.ReadInt32();
            Attributes = reader.Read<MediusWorldAttributesType>();
            MediusWorldUID = reader.ReadInt32();
            ConnectInfo = reader.Read<NetConnectionInfo>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(ApplicationID);
            writer.Write(MaxClients);
            writer.Write(Attributes);
            writer.Write(MediusWorldUID);
            writer.Write(ConnectInfo);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ApplicationID: {ApplicationID} " +
                $"MaxClients: {MaxClients} " +
                $"Attributes: {Attributes} " +
                $"MediusWorldUID: {MediusWorldUID}";
        }
    }
}
