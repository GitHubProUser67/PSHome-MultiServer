using EndianTools.ZipperEndian;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;
using Horizon.ZipperPlugin.Models;
using MultiServerLibrary.Extension;
using Org.BouncyCastle.Math;

namespace Horizon.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(
        NetMessageClass.MessageClassApplication,
        NetMessageTypeIds.NetMessageTypeMAPSHelloMessage
    )]
    public class NetMAPSHelloMessage : BaseMediusPluginMessage
    {
        public override NetMessageTypeIds PacketType =>
            NetMessageTypeIds.NetMessageTypeMAPSHelloMessage;

        public override int Size => (4 * 16) + 10;
        public override ushort ClientBufferSize => (ushort)(Size + 2);
        public override byte PluginId => (byte)NetPluginType.kNetPluginMAPS;

        private readonly int m_transId = 0; // No need for transaction id at this stage.

        public BigInteger RsaPublicKey;

        public bool m_success;
        public bool m_isOnline;
        public CBitset3u m_availableFactions;

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

            BufferImpl.WritePrimitive(buffer, m_success, ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, m_isOnline, ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, m_availableFactions.m_bitArray, ref BitIndex, debug);

            writer.Write(buffer, buffer.Length);
        }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"m_success: {m_success} "
                + $"m_isOnline: {m_isOnline} "
                + $"m_availableFactions: {m_availableFactions.m_bitArray:X8}";
        }
    }
}
