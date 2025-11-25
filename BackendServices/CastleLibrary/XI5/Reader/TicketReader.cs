using EndianTools;
using System;
using System.IO;
using System.Text;
using XI5.Types;

namespace XI5.Reader
{
    public class TicketReader : BinaryReader
    {
        public TicketReader(Stream input) : base(input) { }

        #region Big Endian Conversion

        public override short ReadInt16()
        {
            return EndianAwareConverter.ToInt16(ReadBytes(2), Endianness.BigEndian, 0);
        }

        public override int ReadInt32()
        {
            return EndianAwareConverter.ToInt32(ReadBytes(4), Endianness.BigEndian, 0);
        }

        public override long ReadInt64()
        {
            return EndianAwareConverter.ToInt64(ReadBytes(8), Endianness.BigEndian, 0);
        }

        public override ushort ReadUInt16()
        {
            return EndianAwareConverter.ToUInt16(ReadBytes(2), Endianness.BigEndian, 0);
        }

        public override uint ReadUInt32()
        {
            return EndianAwareConverter.ToUInt32(ReadBytes(4), Endianness.BigEndian, 0);
        }

        public override ulong ReadUInt64()
        {
            return EndianAwareConverter.ToUInt64(ReadBytes(8), Endianness.BigEndian, 0);
        }

        #endregion

        internal TicketVersion ReadTicketVersion() => new TicketVersion((byte)(ReadByte() >> 4), ReadByte());

        internal uint ReadTicketHeader(bool ver40)
        {
            ReadBytes(4);           // header
            return ver40 ? ReadUInt32() : ReadUInt16();    // ticket length
        }

        internal TicketDataSection ReadTicketSectionHeader()
        {
            long position = BaseStream.Position;

            byte sectionHeader = ReadByte();
            if (sectionHeader != 0x30)
                throw new FormatException($"[XI5Ticket] - Expected 0x30 for section header, was {sectionHeader}. Offset is {BaseStream.Position}");

            TicketDataSectionType type = (TicketDataSectionType)ReadByte();
            ushort length = ReadUInt16();

            return new TicketDataSection(type, length, position);
        }

        private TicketData ReadTicketData(TicketDataType expectedType)
        {
            TicketData data = new TicketData((TicketDataType)ReadUInt16(), ReadUInt16());
            if (data.Type != expectedType && expectedType != TicketDataType.Empty)
                throw new FormatException($"[XI5Ticket] - Expected data type to be {expectedType}, was really {data.Type} ({(int)data.Type})");

            return data;
        }

        internal byte[] ReadTicketBinaryData(TicketDataType type = TicketDataType.Binary)
            => ReadBytes(ReadTicketData(type).Length);
        internal string ReadTicketStringData(TicketDataType type = TicketDataType.String)
            => Encoding.Default.GetString(ReadTicketBinaryData(type)).TrimEnd('\0');

        internal uint ReadTicketUInt32Data()
        {
            ReadTicketData(TicketDataType.UInt32);
            return ReadUInt32();
        }

        internal ulong ReadTicketUInt64Data()
        {
            ReadTicketData(TicketDataType.UInt64);
            return ReadUInt64();
        }

        internal DateTimeOffset ReadTicketTimestampData()
        {
            ReadTicketData(TicketDataType.Timestamp);
            return DateTimeOffset.FromUnixTimeMilliseconds((long)ReadUInt64());
        }

        internal void SkipTicketEmptyData(int sections = 1)
        {
            for (int i = 0; i < sections; i++)
                ReadTicketData(TicketDataType.Empty);
        }
    }
}
