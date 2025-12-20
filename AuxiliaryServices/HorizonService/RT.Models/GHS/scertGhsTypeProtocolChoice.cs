using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using Horizon.LIBRARY.Common;

namespace Horizon.RT.Models
{
    /*
    [MediusMessage(GhsOpcode.ghs_ClientProtocolChoice)]
    public class scertGhsTypeProtocolChoice : BaseMediusGHSMessage
    {

        public override GhsOpcode GhsOpcode => GhsOpcode.ghs_ClientProtocolChoice;
        public override ushort msgSize => 0x04;

        public ushort MajorVersion;
        public ushort MinorVersion;

        public override void Deserialize(MessageReader reader)
        {
            MajorVersion = reader.ReadUInt16();
            MinorVersion = reader.ReadUInt16();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(MajorVersion);
            writer.Write(MinorVersion);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MajorVersion: {ReverseBytes16(MajorVersion)} " +
                $"MinorVersion: {ReverseBytes16(MinorVersion)} ";
        }

        #region ReverseBytes16
        /// <summary>
        /// Reverses UInt16 
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public static ushort ReverseBytes16(ushort nValue)
        {
            return (ushort)((ushort)((nValue >> 8)) | (nValue << 8));
        }
        #endregion
    }
    */
}
