using System.Runtime.InteropServices;
using DNSLibrary.Utils;
using EndianTools;
using EndianTools.Marshalling;

namespace DNSLibrary
{
    // 12 bytes message header
    [Endian(Endianness.BigEndian)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Header
    {
        public const int SIZE = 12;

        public static Header FromArray(byte[] header)
        {
            return header.Length < SIZE
                ? throw new ArgumentException("Header length too small")
                : new Header()
                {
                    Id = EndianAwareConverter.ToUInt16(header, Endianness.BigEndian, 0),
                    Flag0 = EndianAwareConverter.ToUInt8(header, Endianness.Automatic, 2),
                    Flag1 = EndianAwareConverter.ToUInt8(header, Endianness.Automatic, 3),
                    QuestionCount = EndianAwareConverter.ToUInt16(header, Endianness.BigEndian, 4),
                    AnswerRecordCount = EndianAwareConverter.ToUInt16(
                        header,
                        Endianness.BigEndian,
                        6
                    ),
                    AuthorityRecordCount = EndianAwareConverter.ToUInt16(
                        header,
                        Endianness.BigEndian,
                        8
                    ),
                    AdditionalRecordCount = EndianAwareConverter.ToUInt16(
                        header,
                        Endianness.BigEndian,
                        10
                    ),
                };
        }

        private ushort id;

        private byte flag0;
        private byte flag1;

        // Question count: number of questions in the Question section
        private ushort qdCount;

        // Answer record count: number of records in the Answer section
        private ushort anCount;

        // Authority record count: number of records in the Authority section
        private ushort nsCount;

        // Additional record count: number of records in the Additional section
        private ushort arCount;

        public int Id
        {
            readonly get { return id; }
            set { id = (ushort)value; }
        }

        public int QuestionCount
        {
            readonly get { return qdCount; }
            set { qdCount = (ushort)value; }
        }

        public int AnswerRecordCount
        {
            readonly get { return anCount; }
            set { anCount = (ushort)value; }
        }

        public int AuthorityRecordCount
        {
            readonly get { return nsCount; }
            set { nsCount = (ushort)value; }
        }

        public int AdditionalRecordCount
        {
            readonly get { return arCount; }
            set { arCount = (ushort)value; }
        }

        public bool Response
        {
            readonly get { return Qr == 1; }
            set { Qr = Convert.ToByte(value); }
        }

        public OperationCode OperationCode
        {
            readonly get { return (OperationCode)Opcode; }
            set { Opcode = (byte)value; }
        }

        public bool AuthorativeServer
        {
            readonly get { return Aa == 1; }
            set { Aa = Convert.ToByte(value); }
        }

        public bool Truncated
        {
            readonly get { return Tc == 1; }
            set { Tc = Convert.ToByte(value); }
        }

        public bool RecursionDesired
        {
            readonly get { return Rd == 1; }
            set { Rd = Convert.ToByte(value); }
        }

        public bool RecursionAvailable
        {
            readonly get { return Ra == 1; }
            set { Ra = Convert.ToByte(value); }
        }

        public bool AuthenticData
        {
            readonly get { return Ad == 1; }
            set { Ad = Convert.ToByte(value); }
        }

        public bool CheckingDisabled
        {
            readonly get { return Cd == 1; }
            set { Cd = Convert.ToByte(value); }
        }

        public ResponseCode ResponseCode
        {
            readonly get { return (ResponseCode)RCode; }
            set { RCode = (byte)value; }
        }

        public static int Size
        {
            get { return SIZE; }
        }

        public readonly byte[] ToArray()
        {
            return Struct.GetBytes(this);
        }

        public override readonly string ToString()
        {
            return ObjectStringifier.New(this).AddAll().Remove(nameof(Size)).ToString();
        }

        // Query/Response Flag
        private byte Qr
        {
            readonly get { return Flag0.GetBitValueAt(7, 1); }
            set { Flag0 = Flag0.SetBitValueAt(7, 1, value); }
        }

        // Operation Code
        private byte Opcode
        {
            readonly get { return Flag0.GetBitValueAt(3, 4); }
            set { Flag0 = Flag0.SetBitValueAt(3, 4, value); }
        }

        // Authorative Answer Flag
        private byte Aa
        {
            readonly get { return Flag0.GetBitValueAt(2, 1); }
            set { Flag0 = Flag0.SetBitValueAt(2, 1, value); }
        }

        // Truncation Flag
        private byte Tc
        {
            readonly get { return Flag0.GetBitValueAt(1, 1); }
            set { Flag0 = Flag0.SetBitValueAt(1, 1, value); }
        }

        // Recursion Desired
        private byte Rd
        {
            readonly get { return Flag0.GetBitValueAt(0, 1); }
            set { Flag0 = Flag0.SetBitValueAt(0, 1, value); }
        }

        // Recursion Available
        private byte Ra
        {
            readonly get { return Flag1.GetBitValueAt(7, 1); }
            set { Flag1 = Flag1.SetBitValueAt(7, 1, value); }
        }

        // Zero (Reserved)
        private byte Z
        {
            readonly get { return Flag1.GetBitValueAt(6, 1); }
#pragma warning disable IDE0251
            set { }
#pragma warning restore IDE0251
        }

        // Authentic Data
        private byte Ad
        {
            readonly get { return Flag1.GetBitValueAt(5, 1); }
            set { Flag1 = Flag1.SetBitValueAt(5, 1, value); }
        }

        // Checking Disabled
        private byte Cd
        {
            readonly get { return Flag1.GetBitValueAt(4, 1); }
            set { Flag1 = Flag1.SetBitValueAt(4, 1, value); }
        }

        // Response Code
        private byte RCode
        {
            readonly get { return Flag1.GetBitValueAt(0, 4); }
            set { Flag1 = Flag1.SetBitValueAt(0, 4, value); }
        }

        private byte Flag0
        {
            readonly get { return flag0; }
            set { flag0 = value; }
        }

        private byte Flag1
        {
            readonly get { return flag1; }
            set { flag1 = value; }
        }
    }
}
