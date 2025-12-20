using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Net;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassDME, MediusDmeMessageIds.ClientConnect)]
    public class TypeClientConnect : BaseDMEMessage
    {
        public override byte PacketType => (byte)MediusDmeMessageIds.ClientConnect;

        public byte ConnectionIndex;
        public byte ConnectedClientIndex;
        public IPAddress PlayerIp;
        public RSA_KEY Key;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            ConnectionIndex = reader.ReadByte();
            ConnectedClientIndex = reader.ReadByte();
            reader.ReadBytes(2);
            PlayerIp = new IPAddress(reader.ReadBytes(4));
            Key = reader.Read<RSA_KEY>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(ConnectionIndex);
            writer.Write(ConnectedClientIndex);
            writer.Write(new byte[2]);
            writer.Write(PlayerIp?.GetAddressBytes() ?? new byte[4]);
            writer.Write(Key);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                 $"ConnectionIndex: {ConnectionIndex} " +
                 $"ConnectedClientIndex: {ConnectedClientIndex} " +
                 $"PlayerIp: {PlayerIp} " +
                 $"Key: {Key}";
        }
    }
}
