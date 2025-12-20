using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.DListSubscription)]
    public class MediusDListSubscription : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.DListSubscription;

        public MessageId MessageID { get; set; }
        public uint fieldMask;
        public uint instanceId;
        public uint relationId;
        public uint filterId;
        public MediusDListID listId;
        public MediusDListServiceLevel level;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            fieldMask = reader.ReadUInt32();
            instanceId = reader.ReadUInt32();
            relationId = reader.ReadUInt32();
            filterId = reader.ReadUInt32();
            listId = reader.Read<MediusDListID>();
            level = reader.Read<MediusDListServiceLevel>();
            reader.ReadBytes(1);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(fieldMask);
            writer.Write(instanceId);
            writer.Write(relationId);
            writer.Write(filterId);
            writer.Write(listId);
            writer.Write(level);
            writer.Write(new byte[1]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"fieldMask: {fieldMask} " +
                $"instanceId: {instanceId} " +
                $"relationId: {relationId} " +
                $"filterId: {filterId} " +
                $"listId: {listId} " +
                $"level: {level}";
        }
    }
}
