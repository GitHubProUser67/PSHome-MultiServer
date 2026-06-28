using EndianTools;

namespace CastleLibrary.S0ny.XI5
{
    internal static class StreamExtensions
    {
        public static uint ReadTicketHeader(this Stream stream, bool isVer40)
        {
            stream.ReadUInt(); // header
            return isVer40 ? stream.ReadUInt() : stream.ReadUShort(); // ticket length
        }

        public static uint ReadUInt(this Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return EndianAwareConverter.ToUInt32(buffer, Endianness.BigEndian, 0);
        }

        public static ulong ReadULong(this Stream stream)
        {
            var buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return EndianAwareConverter.ToUInt64(buffer, Endianness.BigEndian, 0);
        }

        public static ushort ReadUShort(this Stream stream)
        {
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return EndianAwareConverter.ToUInt16(buffer, Endianness.BigEndian, 0);
        }

        public static bool ReadAll(this Stream stream, byte[] buffer, int startIndex, int count)
        {
            if (stream == null)
                return false;

            var offset = 0;
            while (offset < count)
            {
                var readCount = stream.Read(buffer, startIndex + offset, count - offset);
                if (readCount == 0)
                    return false;
                offset += readCount;
            }
            return true;
        }
    }
}
