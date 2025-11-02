using CustomLogger;
using System.Net;
using System.Text;
using System.Net.Sockets;
using QuazalServer.RDVServices.RMC;
using QuazalServer.RDVServices;
using QuazalServer.QNetZ.Factory;
using MultiServerLibrary.Extension;

namespace QuazalServer.QNetZ
{
    public partial class QPacketHandlerPRUDP
	{
		public QPacketHandlerPRUDP(UdpClient udp, uint pid, int port, int BackendPort, string AccessKey, string FactoryIdent, string sourceName = "PRUDP Handler")
		{
            UDP = udp;
            SourceName = sourceName;
			this.AccessKey = AccessKey;
			Factory = (FactoryIdent, ServiceFactoryRDV.TryGetServiceFactory(FactoryIdent) ?? new RMCServiceFactory());
            PID = pid;
			Port = port;
			this.BackendPort = BackendPort;
        }

        private readonly UdpClient? UDP;

        public string SourceName;
		public string AccessKey;
        public (string, RMCServiceFactory) Factory;
        public readonly uint PID;
		public readonly int Port;
        public readonly int BackendPort;
        private readonly List<QPacket> AccumulatedPackets = new();
		private readonly List<QReliableResponse> CachedResponses = new();
		private readonly List<ulong> NATPingTimeToIgnore = new();

		public List<QClient> Clients = new();
		public List<Action> Updates = new();

		public uint ClientIdCounter = 0x12345678; // or client signature

		private QPacket ProcessSYN(QPacket p, IPEndPoint from)
		{
			// create protocol client
			QClient? qclient = GetQClientByEndPointAndSignature(from, p.m_uiSignature);
			qclient ??= NewQClient(from);

            LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] Got SYN packet");
			qclient.SeqCounterOut = 0;

			p.m_uiConnectionSignature = qclient.IDrecv;

			return MakeACK(p, qclient);
        }

		private QPacket ProcessCONNECT(QClient client, QPacket p)
		{
            client.IDsend = p.m_uiConnectionSignature;
			client.State = QClient.StateType.Active;

			LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] Got CONNECT packet");

			var reply = MakeACK(p, client);

			if (p.payload != null && p.payload.Length > 0)
                reply.payload = MakeConnectPayload(client, p);

            return reply;
		}

		private byte[] MakeConnectPayload(QClient client, QPacket p)
		{
			if (p.payload != null)
			{
                MemoryStream m = new(p.payload);

                // read kerberos ticket
                // TODO: decrypt it and read session key instead of using Constants.SessionKey
                uint size = Helper.ReadU32(m);
                byte[] kerberosTicket = new byte[size];
                m.Read(kerberosTicket, 0, (int)size);

                // read encrypted data
                size = Helper.ReadU32(m) - 16;
                byte[] buff = new byte[size];
                m.Read(buff, 0, (int)size);

                buff = Helper.Decrypt(Constants.SessionKey, buff);

                m = new MemoryStream(buff);
                uint userPrincipalID = Helper.ReadU32(m);
                uint connectionId = Helper.ReadU32(m); // TODO: utilize

                // assign player to client and also re-assign new client to player
                PlayerInfo? playerInfo = NetworkPlayers.GetPlayerInfoByPID(userPrincipalID);

                if (playerInfo == null)
                {
                    LoggerAccessor.LogWarn($"[PRUDP Handler] - User pid={userPrincipalID} seem to be dropped but connect was received");
                    return Array.Empty<byte>();
                }

                // drop player in case when of new account but same address
                if (client.PlayerInfo != null)
                    NetworkPlayers.DropPlayerInfo(client.PlayerInfo);

                playerInfo.Client = client;
                client.PlayerInfo = playerInfo;

                uint responseCode = Helper.ReadU32(m);

                // Buffer<uint>
                m = new MemoryStream();
                Helper.WriteU32(m, 4);
                Helper.WriteU32(m, responseCode + 1);

                return m.ToArray();
            }
            
			return Array.Empty<byte>();
		}

		private QPacket ProcessDISCONNECT(QClient client, QPacket p)
		{
			client.State = QClient.StateType.Dropped;

			QPacket reply = MakeACK(p, client);
			reply.m_uiSignature = client.IDsend;

			LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] Got DISCONNECT packet");

			return reply;
		}

		private QPacket ProcessPING(QClient client, QPacket p)
		{
			return MakeACK(p, client);
		}

		public void Send(QPacket reqPacket, QPacket sendPacket, IPEndPoint ep)
		{
			StringBuilder sb = new();

			byte[] data = sendPacket.toBuffer(AccessKey);
			foreach (byte b in data)
				sb.Append(b.ToString("X2") + " ");

			LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] send : {sendPacket.ToStringShort()}");
			LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] send : {sb}");
			LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] send : {sendPacket.ToStringDetailed()}");

			// bufferize in queue then send, that's how Quazal does it
			if (!CacheResponse(reqPacket, sendPacket, ep))
			{
                if (UDP != null)
                    _ = UDP.SendAsync(data, data.Length, ep);
            }

            LoggerAccessor.LogInfo($"[PRUDP Handler] - Packet Data: {data.ToHexString()}");
		}

        public QPacket MakeACK(QPacket p, QClient client)
		{
            return new QPacket(AccessKey, p.toBuffer(AccessKey))
            {
                flags = new List<QPacket.PACKETFLAG>() { QPacket.PACKETFLAG.FLAG_ACK, QPacket.PACKETFLAG.FLAG_HAS_SIZE },

                m_oSourceVPort = p.m_oDestinationVPort,
                m_oDestinationVPort = p.m_oSourceVPort,
                m_uiSignature = client.IDsend,
                payload = Array.Empty<byte>(),
                payloadSize = 0
            };
        }

		public void SendACK(QPacket p, QClient client)
		{
			byte[] payload = MakeACK(p, client).toBuffer(AccessKey);
            if (UDP != null)
                _ = UDP.SendAsync(payload, payload.Length, client.Endpoint);
        }

        public void MakeAndSend(QClient client, QPacket reqPacket, QPacket newPacket, byte[] data)
		{
			using (MemoryStream stream = new(data))
			{
                int numFragments = 0;

                if (stream.Length > Constants.PacketFragmentMaxSize)
                    newPacket.flags?.AddRange(new[] { QPacket.PACKETFLAG.FLAG_HAS_SIZE });

                newPacket.uiSeqId = client.SeqCounterOut;
                newPacket.m_byPartNumber = 1;
                while (stream.Position < stream.Length)
                {
                    int payloadSize = (int)(stream.Length - stream.Position);

                    if (payloadSize <= Constants.PacketFragmentMaxSize)
                        newPacket.m_byPartNumber = 0;  // indicate last packet
                    else
                        payloadSize = Constants.PacketFragmentMaxSize;

                    byte[] buff = new byte[payloadSize];
                    stream.Read(buff, 0, payloadSize);

                    newPacket.uiSeqId++;
                    newPacket.payload = buff;
                    newPacket.payloadSize = (ushort)newPacket.payload.Length;

                    Send(reqPacket, new QPacket(AccessKey, newPacket.toBuffer(AccessKey)), client.Endpoint);

                    newPacket.m_byPartNumber++;
                    numFragments++;
                }

                client.SeqCounterOut = newPacket.uiSeqId;

                LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] sent {numFragments} packets");
            }
        }

		public async void Update()
		{
			await CheckResendPackets();
			await DropClients();

			if (Updates.Count > 0)
			{
                foreach (Action? upd in Updates)
                    upd();
            }
		}

		public void ProcessPacket(byte[] data, IPEndPoint from)
		{
			while (true)
			{
                QPacket packetIn = new(AccessKey, data);
				{
                    using MemoryStream m = new(data);
                    byte[] buff = new byte[(int)packetIn.realSize];
                    m.Read(buff, 0, buff.Length);

                    StringBuilder sb = new();

                    foreach (byte b in data)
                        sb.Append(b.ToString("X2") + " ");

                    LoggerAccessor.LogInfo($"[PRUDP Handler] - Packet Data:{buff.ToHexString()}");

                    LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] received:{packetIn.ToStringShort()}");
                    LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] received:{sb}");
                    LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] received:{packetIn.ToStringDetailed()}");

                    m.Flush();
                }

				QPacket? reply = null;
				QClient? client = null;

				if (packetIn.type != QPacket.PACKETTYPE.SYN && packetIn.type != QPacket.PACKETTYPE.NATPING)
					client = GetQClientByIDrecv(packetIn.m_uiSignature);

				// update client to not time out him
				if (client != null)
                    client.LastPacketTime = DateTime.UtcNow;

                switch (packetIn.type)
				{
					case QPacket.PACKETTYPE.SYN:
                        reply = ProcessSYN(packetIn, from);
						break;
					case QPacket.PACKETTYPE.CONNECT:
						if (client != null && packetIn.flags != null && !packetIn.flags.Contains(QPacket.PACKETFLAG.FLAG_ACK))
						{
							client.sPID = PID;
							client.sPort = (ushort)Port;

							reply = ProcessCONNECT(client, packetIn);
						}
						break;
					case QPacket.PACKETTYPE.DATA:
						{
							// NOT VALID
							if (client == null)
								break;

							if (!Defrag(client, packetIn))
								break;

							// ack for reliable packets
							if (packetIn.flags != null && packetIn.flags.Contains(QPacket.PACKETFLAG.FLAG_ACK))
							{
								OnGotAck(packetIn);
								break;
							}

							// force resend?
							QReliableResponse? cache = GetCachedResponseByRequestPacket(packetIn);
							if (cache != null)
							{
								SendACK(packetIn, client);
								RetrySend(cache);
								break;
							}

							if (packetIn.m_oSourceVPort?.type == QPacket.STREAMTYPE.RVSecure) // Modern Ubisoft games uses Hermes, TODO, implement Hermes
							{
								switch (AccessKey)
								{
									case "ex5LYTJ0":
										if (packetIn.payload != null)
											LoggerAccessor.LogInfo($"[QPakcetHandler] - Client requested a HERMES packet: {packetIn.payload.ToHexString()} - UTF8:{Encoding.UTF8.GetString(packetIn.payload)}");
										else
                                            LoggerAccessor.LogWarn($"[QPakcetHandler] - Client requested a HERMES packet with no data!");
                                        break;
                                    default:
                                        RMC.HandlePacket(this, packetIn, client);
                                        break;
								}
							}

							if (packetIn.m_oSourceVPort?.type == QPacket.STREAMTYPE.DO)
								DO.HandlePacket(this, packetIn, client);
						}
						break;
					case QPacket.PACKETTYPE.DISCONNECT:
						if (client != null)
							reply = ProcessDISCONNECT(client, packetIn);
						break;
					case QPacket.PACKETTYPE.PING:
						if (client != null)
							reply = ProcessPING(client, packetIn);
						break;
					case QPacket.PACKETTYPE.NATPING:

						if (packetIn.payload != null)
						{
                            ulong time = BitConverter.ToUInt64(!BitConverter.IsLittleEndian ? EndianTools.EndianUtils.ReverseArray(packetIn.payload) : packetIn.payload, 5);

                            if (NATPingTimeToIgnore.Contains(time))
                                NATPingTimeToIgnore.Remove(time);
                            else
                            {
                                reply = packetIn;
                                MemoryStream m = new();
                                byte b = (byte)(reply.payload[0] == 1 ? 0 : 1);

                                m.WriteByte(b);

                                Helper.WriteU32(m, 0x1234); //RVCID
                                Helper.WriteU64(m, time);

                                reply.payload = m.ToArray();

                                Send(packetIn, reply, from);

                                m = new MemoryStream();
                                b = (byte)(b == 1 ? 0 : 1);

                                m.WriteByte(b);
                                Helper.WriteU32(m, 0x1234); //RVCID

                                time = Helper.MakeTimestamp();

                                NATPingTimeToIgnore.Add(time);

                                Helper.WriteU64(m, Helper.MakeTimestamp());
                                reply.payload = m.ToArray();
                            }
                        }

						break;
				}

				if (reply != null)
					Send(packetIn, reply, from);

				// more packets in data stream?
				if (packetIn.realSize != data.Length)
				{
					using (MemoryStream m = new(data))
					{
                        int left = (int)(data.Length - packetIn.realSize);
                        byte[] newData = new byte[left];

                        m.Seek(packetIn.realSize, 0);
                        m.Read(newData, 0, left);

                        data = newData;

						m.Flush();
                    }
				}
				else
					break;
			}
		}

		private Task DropClients()
		{
			if (Clients.Count > 0)
			{
                Clients.RemoveAll(client => client == null);
                for (int i = 0; i < Clients.Count; i++)
                {
                    QClient? client = Clients[i];
                    if (client.State == QClient.StateType.Dropped ||
                        client.TimeSinceLastPacket > Constants.ClientTimeoutSeconds)
                    {
                        LoggerAccessor.LogWarn($"[PRUDP Handler] - [{SourceName}] dropping client: 0x{client.IDsend:X8}");
                        client.State = QClient.StateType.Dropped;
                    }
                }
                Clients.RemoveAll(client => client.State == QClient.StateType.Dropped);
            }

			return Task.CompletedTask;
		}

		public QClient? GetQClientByIDrecv(uint id)
		{
			foreach (QClient c in Clients)
			{
				if (c.IDrecv == id)
					return c;
			}

			LoggerAccessor.LogWarn($"[PRUDP Handler] - [{SourceName}] unknown client: 0x{id:X8}");
			return null;
		}

		public QClient NewQClient(IPEndPoint from)
		{
			LoggerAccessor.LogInfo($"[PRUDP Handler] - [{SourceName}] [QUAZAL] New client {from.Address}:{from.Port} registered at server PID={PID}");

            QClient qclient = new(++ClientIdCounter, from);

			Clients.Add(qclient);
			return qclient;
		}

        public QClient? GetQClientByEndPointAndSignature(IPEndPoint ep, uint id)
        {
            foreach (QClient c in Clients)
            {
                if (c.Endpoint.Address.ToString() == ep.Address.ToString() && c.Endpoint.Port == ep.Port && c.IDrecv == id)
                    return c;
            }

            return null;
        }

        public QClient? GetQClientByEndPoint(IPEndPoint ep)
		{
			foreach (QClient c in Clients)
			{
				if (c.Endpoint.Address.ToString() == ep.Address.ToString() && c.Endpoint.Port == ep.Port)
					return c;
			}

			return null;
		}

		public QClient? GetQClientByClientPID(uint userPID)
		{
			foreach (QClient c in Clients)
			{
				if (c.PlayerInfo == null)
					continue;

				// also check if timed out
				if (c.TimeSinceLastPacket > Constants.ClientTimeoutSeconds)
					continue;

				if (c.PlayerInfo.PID == userPID)
					return c;
			}

			return null;
		}
	}
}
