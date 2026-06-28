using EndianTools.ZipperEndian;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;
using MultiServerLibrary.Extension;
using Org.BouncyCastle.Math;

namespace Horizon.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(
        NetMessageClass.MessageClassApplication,
        NetMessageTypeIds.NetMessageTypeUniverseListRequest
    )]
    public class NetMessageUniverseListRequest : BaseMediusPluginMessage
    {
        public override NetMessageTypeIds PacketType =>
            NetMessageTypeIds.NetMessageTypeUniverseListRequest;

        public override int Size => 68;
        public override ushort ClientBufferSize => 0;
        public override byte PluginId => 0;

        public int m_transId;

        public BigInteger RsaPublicKey;

#if DEBUG
        private static readonly bool debug = true;
#else
        private static bool debug = false;
#endif

        public override void DeserializePlugin(MessageReader reader) 
        {
            var BitIndex = 0;
            var rsakey = new uint[16];

            var buffer = reader.ReadBytes(Size);

            for (var i = 0; i < 16; i++)
                BufferImpl.ReadPrimitive(buffer, ref rsakey[i], ref BitIndex, debug);

            BufferImpl.ReadPrimitive(buffer, ref m_transId, ref BitIndex, debug);

            RsaPublicKey = MathUtils.UIntArrayToRsa512Modulus(rsakey);
        }

        public override void SerializePlugin(MessageWriter writer)
        {
            var BitIndex = 0;
            var buffer = new byte[Size];

            // serialize rsa modulus
            // this is sent in maps hello
            // we're going to write the rsa key
            var rsakey = RsaPublicKey.ToByteArrayUnsigned();

            // fix to 64 bytes (512 bit)
            Array.Resize(ref rsakey, 0x40);

            foreach (var val in MathUtils.BigEndianRsa512BytesToUIntArray(rsakey))
                BufferImpl.WritePrimitive(buffer, val, ref BitIndex, debug);

            BufferImpl.WritePrimitive(buffer, m_transId, ref BitIndex, debug);

            writer.Write(buffer, buffer.Length);
        }

        public override string ToString()
        {
            return base.ToString() + " ";
        }
    }
}
