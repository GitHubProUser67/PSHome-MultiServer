using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;

namespace Horizon.RT.Models
{
    public class RawScertMessage : BaseScertMessage
    {

        private readonly RT_MSG_TYPE _id;
        public override RT_MSG_TYPE Id => _id;
        public byte[] Contents { get; set; } = null;

        public RawScertMessage(RT_MSG_TYPE id)
        {
            _id = id;
        }

        #region Serialization

        public override void Deserialize(MessageReader reader)
        {
            Contents = reader.ReadRest();
        }

        public override void Serialize(MessageWriter writer)
        {
            if (Contents != null)
                writer.Write(Contents);
        }

        #endregion

        public override string ToString()
        {
            if (Contents != null)
                return base.ToString() + $" Id:{Id} (0x{(short)Id:X2}) Contents:{BitConverter.ToString(Contents)}";
            else
                return base.ToString() + $" Id:{Id} (0x{(short)Id:X2}) Contents:";
        }
    }
}