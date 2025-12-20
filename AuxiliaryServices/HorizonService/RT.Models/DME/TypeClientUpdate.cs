using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassDME, MediusDmeMessageIds.ClientUpdate)]
    public class TypeClientUpdate : BaseDMEMessage
    {
        public override byte PacketType => (byte)MediusDmeMessageIds.ClientUpdate;

        public byte SourceClientIndex;
        public byte SourceConnectionIndex;
        public short ClientObjectIndex;
        public short NetObjectBufferStart;
        public short NetObjectBufferCount;
        public short NetDataStreamStart;
        public short NetDataStreamCount;
        public long ConnectTime;
        public byte[] unused_field;
        public string Name; //DME_NAME_LENGTH

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            SourceClientIndex = reader.ReadByte();
            SourceConnectionIndex = reader.ReadByte();
            ClientObjectIndex = reader.ReadInt16();
            NetObjectBufferStart = reader.ReadInt16();
            NetObjectBufferCount = reader.ReadInt16();
            NetDataStreamStart = reader.ReadInt16();
            NetDataStreamCount = reader.ReadInt16();
            ConnectTime = reader.ReadUInt32();
            unused_field = reader.ReadBytes(4);
            Name = reader.ReadString(Constants.DME_NAME_LENGTH);
        }

        public override void Serialize(MessageWriter writer)
        { 
            base.Serialize(writer);
 
            writer.Write(SourceClientIndex);
            writer.Write(SourceConnectionIndex);
            writer.Write(ClientObjectIndex);
            writer.Write(NetObjectBufferStart);
            writer.Write(NetObjectBufferCount);
            writer.Write(NetDataStreamStart);
            writer.Write(NetDataStreamCount);
            writer.Write(ConnectTime);
            writer.Write(unused_field);
            writer.Write(Name);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                 $"SourceClientIndex: {SourceClientIndex} " +
                 $"SourceConnectionIndex: {SourceConnectionIndex} " +
                 $"ClientObjectIndex: {ClientObjectIndex} " +
                 $"NetObjectBufferStart: {NetObjectBufferStart} " +
                 $"NetObjectBufferCount: {NetObjectBufferCount} " +
                 $"NetDataStreamStart: {NetDataStreamStart} " +
                 $"NetDataStreamCount: {NetDataStreamCount} " +
                 $"ConnectTime: {ConnectTime} " +
                 $"unused_field: {unused_field} " +
                 $"Name: {Name}";
        }
    }
}
