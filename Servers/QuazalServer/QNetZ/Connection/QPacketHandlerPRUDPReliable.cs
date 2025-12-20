using CustomLogger;
using System.Net;

namespace QuazalServer.QNetZ
{
	public class QPacketState
	{
		public QPacketState(QPacket p)
		{
			Packet = p;
			ReSendCount = 0;
		}
		public QPacket Packet;
		public int ReSendCount;
	}

	public class QReliableResponse
	{
		public QReliableResponse(QPacket srcPacket, IPEndPoint endpoint)
		{
			SrcPacket = srcPacket;
			ResponseList = new List<QPacketState>();
			DropTime = DateTime.UtcNow.AddSeconds(18);
			ResendTime = DateTime.UtcNow;
			Endpoint = endpoint;
		}

		public QPacket SrcPacket;
		public List<QPacketState> ResponseList;

		public DateTime DropTime; // if ACKs not recieved, in this time it will be dropped
		public DateTime ResendTime;
		public IPEndPoint Endpoint; // client endpoint
	}

	public partial class QPacketHandlerPRUDP
	{
        private List<QReliableResponse> reliableResend = new();

        private bool Defrag(QClient client, QPacket packet)
		{
			if (packet.flags != null && packet.flags.Contains(QPacket.PACKETFLAG.FLAG_ACK))
				return true;

			if (packet.flags != null &&!packet.flags.Contains(QPacket.PACKETFLAG.FLAG_RELIABLE))
				return true;

			if (!AccumulatedPackets.Any(x =>
				 x.uiSeqId == packet.uiSeqId &&
				 x.checkSum == packet.checkSum &&
				 x.m_byPartNumber == packet.m_byPartNumber &&
				 x.checkSum == packet.checkSum &&
				 x.m_bySessionID == packet.m_bySessionID &&
				 x.m_uiSignature == packet.m_uiSignature
				))
                AccumulatedPackets.Add(packet);

            if (packet.m_byPartNumber != 0)
                // add and don't process
                return false;

            // got last fragment, assemble
            IEnumerable<QPacket> orderedFragments = AccumulatedPackets.OrderBy(x => x.uiSeqId);
			int numPackets = 0;
			foreach (QPacket fragPacket in orderedFragments)
			{
				numPackets++;
				if (fragPacket.m_byPartNumber == 0)
					break;
			}

			QPacket[] fragments = orderedFragments.Take(numPackets).ToArray();

			// remove fragments that we processed
			AccumulatedPackets.Clear();//RemoveAll(x => fragments.Contains(x));

			if (numPackets > 1)
			{
				// validate algorightm above
				ushort seqId = fragments.First().uiSeqId;
				int nfrag = 1;
				foreach (QPacket fragPacket in fragments)
				{
					if (fragPacket.uiSeqId != seqId)
					{
						LoggerAccessor.LogError("[PRUDP Reliable Handler] - packet sequence mismatch!");
						return false;
					}

					if (fragments.Length == nfrag && fragPacket.m_byPartNumber != 0)
					{
						LoggerAccessor.LogError("[PRUDP Reliable Handler] - packet sequence does not end with 0 - call a programmer!");
						return false;
					}

					if (!(fragPacket.m_byPartNumber == 0 && fragments.Length == nfrag))
					{
						if (fragPacket.m_byPartNumber != nfrag)
						{
							LoggerAccessor.LogError("[PRUDP Reliable Handler] -  insufficient packet fragments - call a programmer!");
							return false;
						}
					}

					seqId++;
					nfrag++;
				}

				// acks are required for each packet
				foreach (QPacket? fragPacket in fragments)
				{
					if (fragPacket.flags != null && fragPacket.flags.Contains(QPacket.PACKETFLAG.FLAG_NEED_ACK))
						SendACK(fragPacket, client);
                }

                MemoryStream fullPacketData = new();
				foreach (QPacket? fragPacket in fragments)
				{
					if (fragPacket.payload != null)
                        fullPacketData.Write(fragPacket.payload.AsSpan());
                }

                // replace packet payload with defragmented data
                packet.payload = fullPacketData.ToArray();
				packet.payloadSize = (ushort)fullPacketData.Length;

				LoggerAccessor.LogInfo($"[PRUDP Reliable Handler] - Defragmented sequence of {numPackets} packets !\n");
			}

			return true;
		}

		// acknowledges packet
		private void OnGotAck(QPacket ackPacket)
		{
			lock (CachedResponses)
			{
				foreach (QReliableResponse cr in CachedResponses)
				{
					cr.ResponseList.RemoveAll(x =>
						x.Packet.m_bySessionID == ackPacket.m_bySessionID &&
						x.Packet.uiSeqId == ackPacket.uiSeqId);
				}

				CachedResponses.RemoveAll(x => !x.ResponseList.Any());
			}
		}

		// returns response cache list by request packet
		private QReliableResponse? GetCachedResponseByRequestPacket(QPacket packet)
		{
			if (packet == null)
				return null;

			if (packet.m_oSourceVPort == null)
			{
				LoggerAccessor.LogError("[PRUDP Reliable Handler] - GetCachedResponseByRequestPacket - invalid packet SRC VPORT!\n");
				return null;
			}

			if (packet.m_oDestinationVPort == null)
			{
				LoggerAccessor.LogError("[PRUDP Reliable Handler] - GetCachedResponseByRequestPacket - invalid packet DEST VPORT!\n");
				return null;
			}

            lock (CachedResponses)
			{
                // delete all invalid messages
                CachedResponses.RemoveAll(x =>
                x.SrcPacket == null ||
                x.SrcPacket.m_oSourceVPort == null ||
                x.SrcPacket.m_oDestinationVPort == null);

                return CachedResponses.FirstOrDefault(cr =>
                        cr.SrcPacket.type == packet.type &&
                        cr.SrcPacket.m_uiSignature == packet.m_uiSignature &&
                        cr.SrcPacket.m_oSourceVPort?.type == packet.m_oSourceVPort.type &&
                        cr.SrcPacket.m_oSourceVPort.port == packet.m_oSourceVPort.port &&
                        cr.SrcPacket.m_oDestinationVPort?.type == packet.m_oDestinationVPort.type &&
                        cr.SrcPacket.m_oDestinationVPort.port == packet.m_oDestinationVPort.port &&
                        cr.SrcPacket.uiSeqId == packet.uiSeqId &&
                        cr.SrcPacket.checkSum == packet.checkSum);
            }
		}

		// Caches the response which is going to be sent
		private bool CacheResponse(QPacket requestPacket, QPacket responsePacket, IPEndPoint ep)
		{
			// only DATA can be reliable
			if (responsePacket.type != QPacket.PACKETTYPE.DATA)
				return false;

			// don't cache non-reliable packets
			if (responsePacket.flags != null && !responsePacket.flags.Contains(QPacket.PACKETFLAG.FLAG_RELIABLE))
				return false;

			if (responsePacket.flags != null && responsePacket.flags.Contains(QPacket.PACKETFLAG.FLAG_ACK))
				return false;

			QReliableResponse? cache = GetCachedResponseByRequestPacket(requestPacket);
			if (cache == null)
			{
                lock (CachedResponses)
				{
                    cache = new QReliableResponse(requestPacket, ep);
                    CachedResponses.Add(cache);
                }
			}
			else
                LoggerAccessor.LogInfo("[PRUDP Reliable Handler] - Found cached request");

            lock (cache.ResponseList)
            {
                cache.ResponseList.Add(new QPacketState(responsePacket));
            }
            return true;
		}

		private void RetrySend(QReliableResponse cache)
		{
			LoggerAccessor.LogWarn("[PRUDP Reliable Handler] - Re-sending reliable packets...");

			int minResendTimes = 0;
			lock (cache.ResponseList)
			{
				Parallel.ForEach(cache.ResponseList, crp =>
				{
					byte[] payload = crp.Packet.toBuffer(AccessKey);

                    if (UDP != null)
                        _ = UDP.SendAsync(payload, payload.Length, cache.Endpoint);

                    crp.ReSendCount++;
                    minResendTimes = Math.Min(minResendTimes, crp.ReSendCount);
                });
			}
			cache.ResendTime = DateTime.UtcNow.AddMilliseconds(Constants.PacketResendTimeSeconds * 1000 + minResendTimes * 250);
		}

		private Task CheckResendPackets()
		{
            QReliableResponse[] reliableResend;
            lock (CachedResponses)
            {
                reliableResend = CachedResponses.Where(x => x.SrcPacket != null && DateTime.UtcNow >= x.ResendTime).ToArray();
                CachedResponses.RemoveAll(x => x.SrcPacket == null || DateTime.UtcNow >= x.DropTime);
            }
            foreach (QReliableResponse crp in reliableResend)
                RetrySend(crp);

            return Task.CompletedTask;
        }
	}
}
