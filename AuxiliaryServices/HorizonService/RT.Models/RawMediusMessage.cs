using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    public class RawMediusMessage : BaseMediusMessage
    {
        protected NetMessageClass _class;
        public override NetMessageClass PacketClass => _class;

        protected byte _messageType;
        public override byte PacketType => _messageType;

        public byte[] Contents { get; set; }

        public RawMediusMessage() { }

        public RawMediusMessage(NetMessageClass msgClass, byte messageType)
        {
            _class = msgClass;
            _messageType = messageType;
        }

        public override void Deserialize(MessageReader reader)
        {
            Contents = reader.ReadRest();
        }

        public override void Serialize(MessageWriter writer)
        {
            if (Contents != null)
                writer.Write(Contents);
        }

        public override string ToString()
        {
            return base.ToString()
                + $" MsgClass:{PacketClass} MsgType:{PacketType} Contents:{BitConverter.ToString(Contents)}";
        }
    }

    public class RawMediusClientMessage : BaseMediusPluginMessage
    {
        protected int _size;
        public override int Size => _size;
        public override ushort ClientBufferSize => throw new NotImplementedException();

        protected NetMessageTypeIds _messageType;
        public override NetMessageTypeIds PacketType => _messageType;

        public byte[] Contents { get; set; }

        public override byte PluginId => throw new NotImplementedException();

        public RawMediusClientMessage() { }

        public RawMediusClientMessage(int size, NetMessageTypeIds messageType)
        {
            _size = size;
            _messageType = messageType;
        }

        public override void DeserializePlugin(MessageReader reader)
        {
            Contents = reader.ReadRest();
        }

        public override void SerializePlugin(MessageWriter writer)
        {
            if (Contents != null)
                writer.Write(Contents);
        }

        public override string ToString()
        {
            return base.ToString()
                + $" MsgType: {PacketType} Contents: {BitConverter.ToString(Contents)}";
        }
    }

    public class RawMediusServerMessage : BaseMediusPluginMessage
    {
        protected int _size;
        public override int Size => _size;

        protected ushort _clientBuffSize;
        public override ushort ClientBufferSize => _clientBuffSize;

        protected byte _pluginId;
        public override byte PluginId => _pluginId;

        protected NetMessageTypeIds _messageType;
        public override NetMessageTypeIds PacketType => _messageType;

        public byte[] Contents { get; set; }

        public RawMediusServerMessage() { }

        public RawMediusServerMessage(
            int size,
            ushort clientBuffSize,
            byte PluginId,
            NetMessageTypeIds messageType
        )
        {
            _pluginId = PluginId;
            _size = size;
            _clientBuffSize = clientBuffSize;
            _messageType = messageType;
        }

        public override void DeserializePlugin(MessageReader reader)
        {
            Contents = reader.ReadRest();
        }

        public override void SerializePlugin(MessageWriter writer)
        {
            if (Contents != null)
                writer.Write(Contents);
        }

        public override string ToString()
        {
            return base.ToString()
                + $" MsgType: {PacketType} Contents: {BitConverter.ToString(Contents)}";
        }
    }
}
