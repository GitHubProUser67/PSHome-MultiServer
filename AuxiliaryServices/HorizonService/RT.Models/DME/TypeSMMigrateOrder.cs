using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassDME, MediusDmeMessageIds.SMMigrateOrder)]
    public class TypeSMMigrateOrder : BaseDMEMessage
    {
        public override byte PacketType => (byte)MediusDmeMessageIds.SMMigrateOrder;

        public long MigrateOrder;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MigrateOrder = reader.ReadUInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MigrateOrder);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MigrateOrder: {MigrateOrder}";
        }
    }
}
