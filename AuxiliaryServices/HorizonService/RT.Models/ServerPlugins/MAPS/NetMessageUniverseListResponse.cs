using EndianTools.ZipperEndian;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;
using MultiServerLibrary.Extension;
using Org.BouncyCastle.Math;

namespace Horizon.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(
        NetMessageClass.MessageClassApplication,
        NetMessageTypeIds.NetMessageTypeUniverseListResponse
    )]
    public class NetMessageUniverseListResponse : BaseMediusPluginMessage
    {
        public override NetMessageTypeIds PacketType =>
            NetMessageTypeIds.NetMessageTypeUniverseListResponse;

        public override int Size => 4096;
        public override ushort ClientBufferSize => 0;
        public override byte PluginId => 0;

        public int m_transId;

        public BigInteger RsaPublicKey;

        public bool m_success;
        public bool m_isLast;
        public string UniverseName = string.Empty;
        public string UniverseAuthDNS = string.Empty;
        public string UniverseAuthIP = string.Empty;
        public string UniverseSvoURL = string.Empty;
        public uint UniversePort;
        public uint UniverseId;

#if DEBUG
        private static readonly bool debug = true;
#else
        private static bool debug = false;
#endif

        public override void DeserializePlugin(MessageReader reader)
        {
            throw new NotImplementedException();
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

            BufferImpl.WritePrimitive(buffer, UniverseId, ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, (ushort)UniverseName.Length, ref BitIndex, debug);

            foreach (char c in UniverseName)
                BufferImpl.WritePrimitive(buffer, (byte)c, ref BitIndex, debug);

            BufferImpl.WritePrimitive(buffer, (ushort)UniverseAuthDNS.Length, ref BitIndex, debug);

            foreach (char c in UniverseAuthDNS)
                BufferImpl.WritePrimitive(buffer, (byte)c, ref BitIndex, debug);

            BufferImpl.WritePrimitive(buffer, (ushort)UniverseAuthIP.Length, ref BitIndex, debug);

            foreach (char c in UniverseAuthIP)
                BufferImpl.WritePrimitive(buffer, (byte)c, ref BitIndex, debug);

            BufferImpl.WritePrimitive(buffer, UniversePort, ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, (ushort)UniverseSvoURL.Length, ref BitIndex, debug);

            foreach (char c in UniverseSvoURL)
                BufferImpl.WritePrimitive(buffer, (byte)c, ref BitIndex, debug);

            BufferImpl.WritePrimitive(buffer, m_success, ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, m_isLast, ref BitIndex, debug);

            writer.Write(buffer, buffer.Length);
        }

        public override string ToString()
        {
            return base.ToString() + " ";
        }
    }
}
