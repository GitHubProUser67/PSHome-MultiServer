using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiSpyService.Data
{
    public class NatNegMessage
    {
        public int Constant; // always 1e 66 6a b2
        public byte ProtocolVersion;
        public byte RecordType;
        public byte[] RecordSpecificData;

        public int ClientId;
        public byte SequenceId; // (0x00 to 0x03)
        public byte Hoststate; // (0x00 for guest, 0x01 for host)
        public byte UseGamePort;
        public string PrivateIPAddress;
        public ushort LocalPort;
        public string GameName;

        public string ClientPublicIPAddress = "127.0.0.1";
        public ushort ClientPublicPort;
        public byte Error;
        public byte GotData;

        public byte PortType; // (0x00, 0x80 or 0x90)
        public byte ReplyFlag;
        public ushort ConnectAckUnknown2;
        public byte ConnectAckUnknown3;
        public int ConnectAckUnknown4;

        public byte NatNegResult;
        public int NatType;
        public int NatMappingScheme;

        public byte ReportAckUnknown1;
        public byte ReportAckUnknown2;
        public ushort ReportAckUnknown3;

        public override string ToString()
        {
            if (RecordType == 0) return "INIT CLIENT " + ClientId + " SEQUENCE " + SequenceId + " HOSTSTATE " + Hoststate + " USEGAMEPORT " + UseGamePort + " PRIVATEIP " + PrivateIPAddress + " LOCALPORT " + LocalPort + " GAMENAME " + GameName;
            if (RecordType == 1) return "INIT_ACK CLIENT " + ClientId + " SEQUENCE " + SequenceId + " HOSTSTATE " + Hoststate;
            if (RecordType == 5) return "CONNECT CLIENT " + ClientId + " CLIENTPUBLICIP " + ClientPublicIPAddress + " CLIENTPUBLICPORT " + ClientPublicPort + " GOTDATA " + GotData + " ERROR " + Error;
            if (RecordType == 6) return "CONNECT_ACK " + ClientId + " PORTTYPE " + PortType + " REPLYFLAG " + ReplyFlag + " UNKNOWN2 " + ConnectAckUnknown2 + " UNKNOWN3 " + ConnectAckUnknown3 + " UNKNOWN4 " + ConnectAckUnknown4;
            if (RecordType == 13) return "REPORT " + ClientId + " PORTTYPE " + PortType + " HOSTSTATE " + Hoststate + " NATNEGRESULT " + NatNegResult + " NATTYPE " + NatType + " NATMAPPINGSCHEME " + NatMappingScheme + " GAMENAME " + GameName;
            if (RecordType == 14) return "REPORT_ACK " + ClientId + " PORTTYPE " + PortType + " UNKNOWN1 " + ReportAckUnknown1 + " UNKNOWN2 " + ReportAckUnknown2 + " NATTYPE " + NatType + " UNKNOWN3 " + ReportAckUnknown3;
            return "RECORDTYPE: " + RecordType;
        }

        public static NatNegMessage ParseData(byte[] bytes)
        {
            if (bytes.Length < 8) return null;
            if (bytes[0] != 0xFD || bytes[1] != 0xFC) return null;
            NatNegMessage msg = new NatNegMessage();
            msg.Constant = _toInt(_getBytes(bytes, 2, 4));
            msg.ProtocolVersion = bytes[6];
            msg.RecordType = bytes[7];
            if (bytes.Length > 8) msg.RecordSpecificData = _getBytes(bytes, 8, bytes.Length - 8);
            if (msg.RecordSpecificData != null)
            {
                if (msg.RecordType == 0)
                {
                    // INIT
                    msg.ClientId = _toInt(_getBytes(msg.RecordSpecificData, 0, 4));
                    msg.SequenceId = msg.RecordSpecificData[4];
                    msg.Hoststate = msg.RecordSpecificData[5];
                    msg.UseGamePort = msg.RecordSpecificData[6];
                    msg.PrivateIPAddress = _toIpAddress(_getBytes(msg.RecordSpecificData, 7, 4));
                    msg.LocalPort = _toShort(_getBytes(msg.RecordSpecificData, 11, 2));
                    msg.GameName = _toString(_getBytes(msg.RecordSpecificData, 13, msg.RecordSpecificData.Length - 13));
                }
                else if (msg.RecordType == 6)
                {
                    // CONNECT_ACK
                    msg.ClientId = _toInt(_getBytes(msg.RecordSpecificData, 0, 4));
                    msg.PortType = msg.RecordSpecificData[4];
                    msg.ReplyFlag = msg.RecordSpecificData[5];
                    msg.ConnectAckUnknown2 = _toShort(_getBytes(msg.RecordSpecificData, 6, 2));
                    msg.ConnectAckUnknown3 = msg.RecordSpecificData[8];
                    msg.ConnectAckUnknown4 = _toInt(_getBytes(msg.RecordSpecificData, 9, 4));
                }
                else if (msg.RecordType == 13)
                {
                    // CONNECT_ACK
                    msg.ClientId = _toInt(_getBytes(msg.RecordSpecificData, 0, 4));
                    msg.PortType = msg.RecordSpecificData[4];
                    msg.Hoststate = msg.RecordSpecificData[5];
                    msg.NatNegResult = msg.RecordSpecificData[6];
                    msg.NatType = _toIntBigEndian(_getBytes(msg.RecordSpecificData, 7, 4));
                    msg.NatMappingScheme = _toIntBigEndian(_getBytes(msg.RecordSpecificData, 11, 4));
                    msg.GameName = _toString(_getBytes(msg.RecordSpecificData, 15, msg.RecordSpecificData.Length - 15));
                }
            }
            return msg;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>
            {
                0xFD,
                0xFC,
                0x1E,
                0x66,
                0x6a,
                0xb2,
                ProtocolVersion,
                RecordType
            };
            _addInt(bytes, ClientId);
            if (RecordType == 1)
            {
                // INIT_ACK (0x01)
                bytes.Add(SequenceId);
                bytes.Add(Hoststate);
                _addBytes(bytes, 0xFF, 0xFF); //ff ff		unknown_5. Always 0xffff
                _addBytes(bytes, 0x6d, 0x16, 0xb5, 0x7d, 0xea); //	unknown_6. Any useless constant (also for other games).
            }
            else if (RecordType == 5)
            {
                // CONNECT (0x05)
                _addIPAddress(bytes, ClientPublicIPAddress);
                _addShort(bytes, ClientPublicPort);
                bytes.Add(GotData);
                bytes.Add(Error);
            }
            else if (RecordType == 14)
            {
                // REPORT_ACK
                bytes.Add(PortType);
                bytes.Add(ReportAckUnknown1);
                bytes.Add(ReportAckUnknown2);
                _addInt(bytes, NatType);
                _addShort(bytes, ReportAckUnknown3);
            }
            return bytes.ToArray();
        }

        private static string _toString(byte[] bytes) {
            if (bytes == null) return null;
            List<byte> bs = new List<byte>();
            for (int i = 0; i < bytes.Length && bytes[i] > 0; i++)
                bs.Add(bytes[i]);
            return Encoding.ASCII.GetString(bs.ToArray());
        }

        private static void _addBytes(List<byte> bytes, params byte[] adds)
        {
            bytes.AddRange(adds);
        }

        private static void _addInt(List<byte> bytes, int value)
        {
            List<byte> b = new List<byte>(BitConverter.GetBytes((int)value));
            if (BitConverter.IsLittleEndian)
                b.Reverse();
            while (b.Count < 4) b.Insert(0, 0);
            bytes.AddRange(b);
        }

        private static void _addShort(List<byte> bytes, ushort value)
        {
            List<byte> b = new List<byte>(BitConverter.GetBytes(value));
            if (BitConverter.IsLittleEndian)
                b.Reverse();
            while (b.Count < 2) b.Insert(0, 0);
            bytes.AddRange(b);
        }

        private static void _addIPAddress(List<byte> bytes, string address)
        {
            bytes.AddRange(address.Split('.').Select((b) => { return (byte)Convert.ToInt32(b); }));
        }

        private static int _toInt(byte[] bytes)
        {
            if (bytes == null) return -1;
            return (int)bytes[0] * 256 * 256 * 256 + (int)bytes[1] * 256 * 256 + (int)bytes[2] * 256 + bytes[3];
        }

        private static int _toIntBigEndian(byte[] bytes)
        {
            if (bytes == null) return -1;
            return (int)bytes[3] * 256 * 256 * 256 + (int)bytes[2] * 256 * 256 + (int)bytes[1] * 256 + bytes[0];
        }

        private static ushort _toShort(byte[] bytes)
        {
            if (bytes == null) return 0;
            return (ushort)(bytes[0] * 256 + bytes[1]);
        }

        public static string _toIpAddress(byte[] bytes) {
            if (bytes == null) return null;
            return string.Join(".", bytes.Select((b) => { return b.ToString(); }));
        }

        private static byte[] _getBytes(byte[] bytes, int index, int nofBytes) {
            if (bytes == null) return null;
            byte[] result = new byte[nofBytes];
            for (int i = 0; i < nofBytes; i++) {
                if (index+i >= bytes.Length) result[i] = 0; else result[i] = bytes[index+i];
            }
            return result;
        }
    }
}
