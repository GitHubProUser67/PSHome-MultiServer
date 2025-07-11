using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using EndianTools.ZipperEndian;
using Horizon.RT.Models;
using HorizonService.ZipperPlugin.Models;
using Org.BouncyCastle.Math;
using System;
using EndianTools;

namespace HorizonService.RT.Models.ServerPlugins.MAPS
{
    [MediusMessage(NetMessageClass.MessageClassApplication, NetMessageTypeIds.NetMessageTypeMAPSHelloMessage)]
    public class NetMAPSHelloMessage : BaseApplicationMessage
    {
        public override NetMessageTypeIds PacketType => NetMessageTypeIds.NetMessageTypeMAPSHelloMessage;

        public override byte IncomingMessage => 0;
        public override int Size => 8;
        public override byte PluginId => 31;

        public BigInteger RsaPublicKey = BigInteger.Zero;
        public bool m_success;
        public bool m_isOnline;
        public CBitset3u m_availableFactions;
#if DEBUG
        private static bool debug = true;
#else
        private static bool debug = false;
#endif
        public override void DeserializePlugin(MessageReader reader)
        {
            throw new NotImplementedException();
        }

        public override void SerializePlugin(MessageWriter writer)
        {
            int BitIndex = 0;
            int statusBitIndex = 0;
            byte[] buffer = new byte[64 + 4 + 4];
            byte[] statusBuffer = new byte[4];

            BufferImpl.WritePrimitive(statusBuffer, m_success ? (byte)1 : (byte)0, ref statusBitIndex, debug);
            BufferImpl.WritePrimitive(statusBuffer, m_isOnline ? (byte)1 : (byte)0, ref statusBitIndex, debug);

            // serialize rsa modulus
            // this is sent in server hello at offset 0x194
            // we're going to overwrite the cert at that offset to store the rsa modulus
            var rsakey = RsaPublicKey.ToByteArrayUnsigned();

            // fix to 64 bytes (512 bit)
            Array.Resize(ref rsakey, 0x40);

            uint[] key = BigEndianBytesToUIntArray(rsakey);

            foreach (uint val in key)
            {
                BufferImpl.WritePrimitive(buffer, val, ref BitIndex, debug);
            }

            BufferImpl.WritePrimitive(buffer, EndianAwareConverter.ToInt32(statusBuffer, Endianness.LittleEndian, 0), ref BitIndex, debug);
            BufferImpl.WritePrimitive(buffer, m_availableFactions.m_bitArray, ref BitIndex, debug);

            writer.Write(buffer, buffer.Length);
        }

        private static uint[] BigEndianBytesToUIntArray(byte[] bigEndianBytes)
        {
            if (bigEndianBytes.Length != 64)
                throw new ArgumentException("Array length must be 64 bytes.");

            uint[] result = new uint[16];

            for (int i = 0; i < result.Length; i += 4)
            {
                result[i] = EndianAwareConverter.ToUInt32(bigEndianBytes, Endianness.LittleEndian, (uint)i);
            }

            return result;
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"m_success: {m_success} " +
                $"m_isOnline: {m_isOnline} " +
                $"m_availableFactions: {m_availableFactions}";
        }
    }
}
