using System.Text;

namespace QuazalServer.QNetZ
{
	public class KerberosTicket
	{
		public uint userPID;
		public byte[]? sessionKey;
		public uint serverPID;
		public byte[]? ticket;

		public KerberosTicket(uint pid, uint sPID, byte[] sesKey, byte[] ticketData)
		{
			userPID = pid;
			serverPID = sPID;
			sessionKey = sesKey;
			ticket = ticketData;
		}

        public byte[]? ToBuffer(string AccessKey, string? input = null)
		{
			if (sessionKey != null && ticket != null)
			{
                using (MemoryStream ms = new())
                {
                    ms.Write(sessionKey, 0, 16);

                    Helper.WriteU32(ms, serverPID);
                    Helper.WriteU32(ms, (uint)ticket.Length);

                    ms.Write(ticket, 0, ticket.Length);

                    byte[] key;
                    byte[] buff = ms.ToArray();

                    switch (AccessKey)
                    {
                        case "os4R9pEiy":
                        case "OLjNg84Gh":
                        case "cYoqGd4f":
                        case "QusaPha9":
                        case "q1UFc45UwoyI":
                        case "os1oBiCa2bPv":
                        case "h0rszqTw":
                        case "lON6yKGp":
                        case "4TeVtJ7V":
                        case "HJb8Ix1M":
                        case "b417OVR":
                        case "7aK4858Q":
                        case "asdd3$#a":
                        case "oTyiaY4Ks":
                        case "HgBfd54p":
                        case "bR8fafEw":
                        case "tFkQh5ds":
						case "uG9Kv3p":
                        case "Ey6Ma18":
                        case "bfa620c57c2d3bcdf4362a6fa6418e58":
						case "bf99796d6674ef63697f453296d7934c":
                        case "88808fd91016c8f2ce3670ca1216a113":
                            key = Helper.DeriveKey(userPID, input ?? "PS3NPDummyPwd");
                            break;
						case "d52d1e000328fbc724fde65006b88b56":
                            key = Helper.DeriveKey(userPID, input ?? "LSP PASSWORD");
                            break;
                        case "w6kAtr3T":
                            key = Helper.DeriveKey(userPID, input ?? "UbiDummyPwd");
                            break;
                        default:
                            key = Helper.DeriveKey(userPID, input ?? "h7fyctiuucf");
                            break;
                    }

                    buff = Helper.Encrypt(key, buff);

                    byte[] hmac = Helper.MakeHMAC(key, buff);

                    using (MemoryStream ms1 = new())
                    {
                        ms1.Write(buff, 0, buff.Length);
                        ms1.Write(hmac, 0, hmac.Length);

                        return ms1.ToArray();
                    }
                }
            }

			return null;
		}

        public override string ToString()
		{
			StringBuilder sb = new();
			sb.AppendLine("\t[Kerberos Ticket]");

			sb.Append("\t\t[Session Key : { ");
			if (sessionKey != null)
            {
                foreach (byte b in sessionKey)
                    sb.Append(b.ToString("X2") + " ");
            }
            sb.AppendLine("}]");

			sb.AppendLine("\t\t[Server PID : 0x" + serverPID.ToString("X8"));

			sb.Append("\t\t[Ticket Data : { ");
			if (ticket != null)
			{
                foreach (byte b in ticket)
                    sb.Append(b.ToString("X2") + " ");
            }
			sb.AppendLine("}]");

			return sb.ToString();
		}
	}
}
