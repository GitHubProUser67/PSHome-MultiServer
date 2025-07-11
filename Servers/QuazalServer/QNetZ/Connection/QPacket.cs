using MultiServerLibrary.Extension;
using EndianTools;
using System.Text;

namespace QuazalServer.QNetZ
{
    public class QPacket
	{
		public enum STREAMTYPE
		{
			DO = 1,
			RVAuthentication,
			RVSecure,
			SandBoxMgmt,
			NAT,
			SessionDiscovery,
			NATEcho,
			Routing,
			LastStreamType
		}

		public enum PACKETFLAG
		{
			FLAG_ACK = 1,
			FLAG_RELIABLE = 2,
			FLAG_NEED_ACK = 4,
			FLAG_HAS_SIZE = 8,
			FLAG_FLOODED = 16
		}

		public enum PACKETTYPE
		{
			SYN,
			CONNECT,
			DATA,
			DISCONNECT,
			PING,
			NATPING
		}

		public class VPort
		{
			public STREAMTYPE type;
			public byte port;
			public VPort(byte b)
			{
				type = (STREAMTYPE)(b >> 4);
				port = (byte)(b & 0xF);
			}

			public override string ToString()
			{
				return "VPort[port=" + port.ToString("D2") + " type=" + type + "]";
			}

			public byte toByte()
			{
				byte result = port;
				result |= (byte)((byte)type << 4);
				return result;
			}

			public override bool Equals(object? obj)
			{
				//Check for null and compare run-time types.
				if ((obj == null) || !GetType().Equals(obj.GetType()))
					return false;

				var p = (VPort)obj;
				return (type == p.type) && (port == p.port);
			}

			public override int GetHashCode()
			{
				return ((int)type << 2) ^ port;
			}
		}

		public VPort? m_oSourceVPort;
		public VPort? m_oDestinationVPort;
		public byte m_byPacketTypeFlags;
		public PACKETTYPE type;
		public List<PACKETFLAG>? flags;
		public byte m_bySessionID;
		public uint m_uiSignature;
		public ushort uiSeqId;
		public uint m_uiConnectionSignature;
		public byte m_byPartNumber;
		public ushort payloadSize;
		public byte[]? payload;
		public byte[]? checkSum;
		public bool usesCompression = true;
		public uint realSize;
		public uint Port;
		private bool NewerCheckum;

        public QPacket(string AccessKey)
        {
            switch (AccessKey)
            {
                case "ex5LYTJ0":
                    NewerCheckum = true;
                    break;

            }
        }

        public QPacket(string AccessKey, byte[] data)
			: this(AccessKey, new MemoryStream(data))
		{
			switch (AccessKey)
			{
				case "ex5LYTJ0":
					NewerCheckum = true;
					break;

            }
		}

		public QPacket(string AccessKey, Stream stream)
		{
            switch (AccessKey)
            {
                case "ex5LYTJ0":
                    NewerCheckum = true;
                    break;

            }

            m_oSourceVPort = new VPort(Helper.ReadU8(stream));
			m_oDestinationVPort = new VPort(Helper.ReadU8(stream));
			m_byPacketTypeFlags = Helper.ReadU8(stream);
			type = (PACKETTYPE)(m_byPacketTypeFlags & 0x7);
			flags = new List<PACKETFLAG>();

			ExtractFlags();

			m_bySessionID = Helper.ReadU8(stream);
			m_uiSignature = Helper.ReadU32(stream);
			uiSeqId = Helper.ReadU16(stream);

			if (type == PACKETTYPE.SYN || type == PACKETTYPE.CONNECT)
				m_uiConnectionSignature = Helper.ReadU32(stream);

			if (type == PACKETTYPE.DATA)
				m_byPartNumber = Helper.ReadU8(stream);

			if (flags.Contains(PACKETFLAG.FLAG_HAS_SIZE))
				payloadSize = Helper.ReadU16(stream);
			else
				payloadSize = (ushort)(stream.Length - stream.Position - (NewerCheckum ? 4 : 1));

			using (MemoryStream pl = new())
			{
                if (payloadSize != 0)
                    for (int i = 0; i < payloadSize; i++)
                        pl.WriteByte(Helper.ReadU8(stream));

                payload = pl.ToArray();
            }

			if (payload != null && payload.Length > 0 && type != PACKETTYPE.SYN && m_oSourceVPort.type != STREAMTYPE.NAT)
			{
				if (m_oSourceVPort.type == STREAMTYPE.RVSecure && AccessKey != "hg7j1") // TDU PS2 Beta has no encryption at all!
					payload = Helper.Decrypt(Constants.KeyDATA, payload);

				usesCompression = payload[0] != 0;

				if (usesCompression)
				{
					using (MemoryStream m2 = new())
					{
                        m2.Write(payload, 1, payload.Length - 1);
                        payload = Helper.Decompress(AccessKey, m2.ToArray());
                    }
				}
				else
				{
					using (MemoryStream m2 = new())
					{
                        m2.Write(payload, 1, payload.Length - 1);
                        payload = m2.ToArray();
                    }
				}

				payloadSize = (ushort)payload.Length;
			}

			checkSum = NewerCheckum ? new byte[4] { Helper.ReadU8(stream), Helper.ReadU8(stream), Helper.ReadU8(stream), Helper.ReadU8(stream) } : new byte[1] { Helper.ReadU8(stream) };
			realSize = (uint)stream.Position;
		}

		public byte[]? getProcessedPayload(string AccessKey)
		{
			byte[]? tmpPayload = payload;

			if (tmpPayload != null && tmpPayload.Length > 0 && type != PACKETTYPE.SYN && m_oSourceVPort?.type != STREAMTYPE.NAT)
			{
				if (usesCompression && AccessKey != "hg7j1" && AccessKey != "yh64s" && AccessKey != "uG9Kv3p" && AccessKey != "1WguH+y") // Old LZO based clients not need compressed response.
				{
					uint sizeBefore = (uint)tmpPayload.Length;
					byte[] buff = Helper.Compress(tmpPayload);
					byte count = (byte)(sizeBefore / buff.Length);

					if ((sizeBefore % buff.Length) != 0)
						count++;

					using (MemoryStream m2 = new())
					{
                        m2.WriteByte(count);
                        m2.Write(buff, 0, buff.Length);
                        tmpPayload = m2.ToArray();
                    }
				}
				else
				{
					using (MemoryStream m2 = new())
					{
                        m2.WriteByte(0);
                        m2.Write(tmpPayload, 0, tmpPayload.Length);
                        tmpPayload = m2.ToArray();
                    }
				}

				if (m_oSourceVPort?.type == STREAMTYPE.RVSecure && AccessKey != "hg7j1") // TDU PS2 Beta has no encryption at all!
					tmpPayload = Helper.Encrypt(Constants.KeyDATA, tmpPayload);
			}

			return tmpPayload;
		}

		public byte[] toBuffer(string AccessKey)
		{
			// process type flags
			byte typeFlag = (byte)type;

			if (flags != null)
			{
				foreach (PACKETFLAG flag in flags)
					typeFlag |= (byte)((byte)flag << 3);
			}

			// write
			using (MemoryStream m = new())
			{
                if (m_oSourceVPort != null)
                    Helper.WriteU8(m, m_oSourceVPort.toByte());
                if (m_oDestinationVPort != null)
                    Helper.WriteU8(m, m_oDestinationVPort.toByte());
                Helper.WriteU8(m, typeFlag);
                Helper.WriteU8(m, m_bySessionID);
                Helper.WriteU32(m, m_uiSignature);
                Helper.WriteU16(m, uiSeqId);

                if (type == PACKETTYPE.SYN || type == PACKETTYPE.CONNECT)
                    Helper.WriteU32(m, m_uiConnectionSignature);

                if (type == PACKETTYPE.DATA)
                    Helper.WriteU8(m, m_byPartNumber);

                // compress
                byte[]? processedPayload = getProcessedPayload(AccessKey);

                if (processedPayload != null)
                {
                    if (flags != null && flags.Contains(PACKETFLAG.FLAG_HAS_SIZE))
                        Helper.WriteU16(m, (ushort)processedPayload.Length);

                    m.Write(processedPayload, 0, processedPayload.Length);

                    return AddCheckSum(m.ToArray(), AccessKey, NewerCheckum, ref checkSum);
                }
            }

			return Array.Empty<byte>();
		}

		public static byte[] AddCheckSum(byte[] buff, string AccessKey, bool NewerCheckum, ref byte[]? checkSum)
		{
			try
			{
                byte[] result = new byte[buff.Length + (NewerCheckum ? 4 : 1)];

                for (int i = 0; i < buff.Length; i++)
                    result[i] = buff[i];

                if (NewerCheckum)
                {
                    checkSum = MakeChecksum4(buff, AccessKey);
                    Buffer.BlockCopy(checkSum, 0, result, result.Length - 4, 4);
                }
                else
                {
                    checkSum = new byte[1] { MakeChecksum(buff, AccessKey) };
                    result[buff.Length] = checkSum[0];
                }

                return result;
            }
			catch (Exception ex)
			{
				CustomLogger.LoggerAccessor.LogError($"[QPacket] - AddCheckSum failed with exception:{ex}");
			}


			return buff;
		}

		private static byte GetProtocolSetting(byte proto, string AccessKey)
		{
			switch (proto)
			{
				case 3:
					if (AccessKey.Length == 4 && AccessKey.Contains("0x") && byte.TryParse(AccessKey, out byte Protocol))
						return Protocol;
					else
						return (byte)Encoding.ASCII.GetBytes(AccessKey).Sum(b => b);
				case 1:
				case 5:
				default:
					return 0;
			}
		}

		private static byte MakeChecksum(byte[] data, string AccessKey, byte setting = 0xFF)
		{
			if (setting == 0xFF)
				setting = GetProtocolSetting((byte)(data[0] >> 4), AccessKey);

			bool isBigEndian = !BitConverter.IsLittleEndian;
            uint tmp = 0;
			for (int i = 0; i < data.Length / 4; i++)
				tmp += BitConverter.ToUInt32(isBigEndian ? EndianUtils.ReverseArray(data) : data, i * 4);

			uint leftOver = (uint)data.Length & 3;
			uint processed = 0;
			byte tmp2 = 0, tmp3 = 0, tmp4 = 0;
			uint pos = (uint)data.Length - leftOver;

			if (leftOver >= 2)
			{
				processed = 2;
				tmp2 = data[pos];
				tmp3 = data[pos + 1];
				pos += 2;
			}

			if (processed >= leftOver)
				tmp4 = setting;
			else
				tmp4 = (byte)(setting + data[pos]);

			return (byte)((byte)(tmp >> 24) +
				   (byte)(tmp >> 16) +
				   (byte)(tmp >> 8) +
				   (byte)tmp + tmp2 + tmp3 + tmp4);
		}

		private static uint Key4(string AccessKey)
		{
            // Check if the access key length is multiple of 4
            if (AccessKey.Length % 4 != 0)
                throw new ArgumentException("Access key length must be a multiple of 4.");

            uint keySum = 0;

            for (int i = 0; i < AccessKey.Length; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    keySum += (uint)AccessKey[i + j] << (j * 8);
                }
            }

            return keySum;
        }

		public static byte[] MakeChecksum4(byte[] data, string AccessKey)
        {
			bool LittleEndian = BitConverter.IsLittleEndian;
            int len = data.Length;
            len -= len % 4;

			uint dataSum;
            using (MemoryStream ms = new(data.Take(len).ToArray()))
			using (BinaryReader reader = new(ms))
			{
                dataSum = reader.ReadUInt32();
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    dataSum += reader.ReadUInt32();
                }
            }

            // copying trailing bytes
            byte[] trailerBytes = new byte[4];
            for (int i = 0; i < data.Length - len; i++)
            {
                trailerBytes[i] = data[len + i];
            }

			if (!LittleEndian)
				Array.Reverse(trailerBytes);

			uint Checksum = dataSum + Key4(AccessKey) + BitConverter.ToUInt32(trailerBytes, 0);

            return BitConverter.GetBytes(!LittleEndian ? EndianUtils.ReverseUint(Checksum) : Checksum);
        }

        private void ExtractFlags()
		{
			byte v = (byte)(m_byPacketTypeFlags >> 3);
			int[] values = (int[])Enum.GetValues(typeof(PACKETFLAG));
			for (int i = 0; i < values.Length; i++)
				if ((v & values[i]) != 0)
					flags?.Add((PACKETFLAG)values[i]);
		}

		public string GetFlagsString()
		{
			StringBuilder sb = new();
			if (flags != null)
			{
                foreach (PACKETFLAG flag in flags)
                    sb.Append("[" + flag.ToString().Replace("FLAG_", string.Empty) + "]");
            }
			return sb.ToString();
		}

		public string ToStringDetailed()
		{
			StringBuilder sb = new();
			sb.AppendLine(" UDPPacket {");
			sb.AppendLine(" From:" + m_oSourceVPort);
			sb.AppendLine(" To:" + m_oDestinationVPort);
			sb.AppendLine(" Flags:" + GetFlagsString());
			sb.AppendLine(" Type:" + type);
			sb.AppendLine(" Session ID:0x" + m_bySessionID.ToString("X2"));
			sb.AppendLine(" Signature:0x" + m_uiSignature.ToString("X8"));
			sb.AppendLine(" Sequence ID:0x" + uiSeqId.ToString("X4"));
			if (type == PACKETTYPE.SYN || type == PACKETTYPE.CONNECT)
				sb.AppendLine(" Conn. Sig.:0x" + m_uiConnectionSignature.ToString("X8"));
			if (type == PACKETTYPE.DATA)
				sb.AppendLine(" Part Number:0x" + m_byPartNumber.ToString("X2"));
			if (flags != null && flags.Contains(PACKETFLAG.FLAG_HAS_SIZE))
				sb.AppendLine(" Payload Size:0x" + payloadSize.ToString("X4"));
			sb.Append(" PayLoad:");
			if (payload != null)
			{
				foreach (byte b in payload)
					sb.Append(b.ToString("X2") + " ");
			}
			sb.AppendLine();
			sb.AppendLine(" Checksum:0x" + (checkSum ?? Array.Empty<byte>()).ToHexString());
			sb.AppendLine("}");
			return sb.ToString();
		}

		public string ToStringShort()
		{
			return "UDPPacket { " + type + " ( " + GetFlagStringShort() + " )}";
		}

		private string GetFlagStringShort()
		{
			string s = string.Empty;
			if (flags != null)
			{
                s += flags.Contains(PACKETFLAG.FLAG_RELIABLE) ? "R" : " ";
                s += flags.Contains(PACKETFLAG.FLAG_ACK) ? "A" : " ";
                s += flags.Contains(PACKETFLAG.FLAG_NEED_ACK) ? "W" : " ";
                s += flags.Contains(PACKETFLAG.FLAG_HAS_SIZE) ? "S" : " ";
            }
			return s;
		}
	}
}
