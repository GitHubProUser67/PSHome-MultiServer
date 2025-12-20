using System.IO;
namespace Horizon.RT.Models
{
    /*
    [MediusMessage(GhsOpcode.ghs_ServerProtocolNegotiation)]
    public class scertGhsTypeVersionRequest : BaseMediusGHSMessage
    {

        public override GhsOpcode GhsOpcode => GhsOpcode.ghs_ServerProtocolNegotiation;
        public override ushort msgSize => 0x04;

        //public ushort messageEcho;
        public ushort maxMajorVersion;
        public ushort maxMinorVersion;
        //public ushort minMajorVersion;
        //public ushort minMinorVersion;

        public override void Deserialize(MessageReader reader)
        {
            //messageEcho = reader.ReadUInt16();
            maxMajorVersion = reader.ReadUInt16();
            maxMinorVersion = reader.ReadUInt16();
            //minMajorVersion = reader.ReadUInt16();
            //minMinorVersion = reader.ReadUInt16();
        }

        public override void Serialize(MessageWriter writer)
        {
            //writer.Write(messageEcho);
            writer.Write(maxMajorVersion);
            writer.Write(maxMinorVersion);
            //writer.Write(minMajorVersion);
            //writer.Write(minMinorVersion);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"maxMajorVersion: {ReverseBytes16(maxMajorVersion)} " +
                $"maxMinorVersion: {ReverseBytes16(maxMinorVersion)} ";
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
