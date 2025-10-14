using CustomLogger;
using MultiSpyService.Data;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace MultiSpy.Servers
{
    internal class ServerNatNeg
	{
		public Thread Thread;

		private const int BufferSize = 65535;
		private Socket? _socket;
		private SocketAsyncEventArgs? _socketReadEvent;
		private byte[]? _socketReceivedBuffer;
        private ConcurrentDictionary<int, NatNegClient> _Clients = new ConcurrentDictionary<int,NatNegClient>();

		public ServerNatNeg(IPAddress listen, ushort port)
		{
			Thread = new Thread(StartServer) {
				Name = "Server NatNeg Socket Thread"
			};
			Thread.Start(new AddressInfo() {
				Address = listen,
				Port = port
			});
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
            if (disposing)
            {
                if (_socket != null)
                {
                    try
                    {
                        _socket.Close();
                        _socket.Dispose();
                    }
                    catch { /* ignore */ }
                    _socket = null;
                }
            }
        }

		~ServerNatNeg()
		{
			Dispose(false);
		}

		private void StartServer(object? parameter)
		{
			AddressInfo? info = (AddressInfo?)parameter;
			LoggerAccessor.LogInfo("[ServerNatNeg] - Starting Nat Neg Listener");
			try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {
					SendTimeout = 5000,
					ReceiveTimeout = 5000,
					SendBufferSize = BufferSize,
					ReceiveBufferSize = BufferSize
				};

				_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, true);
				_socket.Bind(new IPEndPoint(info.Address, info.Port));

				_socketReadEvent = new SocketAsyncEventArgs() {
					RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0)
				};
				_socketReceivedBuffer = new byte[BufferSize];
				_socketReadEvent.SetBuffer(_socketReceivedBuffer, 0, BufferSize);
				_socketReadEvent.Completed += OnDataReceived;
			} catch (Exception e) {
                LoggerAccessor.LogError("[ServerNatNeg] - " + String.Format("Unable to bind Server List Reporting to {0}:{1}", info.Address, info.Port));
                LoggerAccessor.LogError("[ServerNatNeg] - " + e.ToString());
				return;
			}

			WaitForData();
		}

		private void WaitForData()
		{
			Thread.Sleep(10);
			GC.Collect();

			try {
				_socket?.ReceiveFromAsync(_socketReadEvent);
			} catch (SocketException e) {
                LoggerAccessor.LogError("[ServerNatNeg] - " + "Error receiving data");
                LoggerAccessor.LogError("[ServerNatNeg] - " + e.ToString());
				return;
			}
		}

		private void OnDataReceived(object? sender, SocketAsyncEventArgs e)
		{
            /*
             * Connection Protocol
             * 
             * From http://wiki.tockdom.com/wiki/MKWii_Network_Protocol/Server/mariokartwii.natneg.gs.nintendowifi.net
             * 
             * The NATNEG communication to enable a peer to peer communication is is done in the following steps:
             * 
             * Both clients (called guest and host to distinguish them) exchange an unique natneg-id. In all observed Wii games this communication is done using Server MS and Server MASTER.
             * Both clients sends independent of each other a sequence of 4 INIT packets to the NATNEG servers. The sequence number goes from 0 to 3. The guest sets the host_flag to 0 and the host to 1. The natneg-id must be the same for all packets.
             * Packet 0 (sequence number 0) is send from the public address to server NATNEG1. This public address is later used for the peer to peer communication.
             * Packet 1 (sequence number 1) is send from the communication address (usually an other port than the public address) to server NATNEG1.
             * Packet 2 (sequence number 2) is send from the communication address to server NATNEG2 (any kind of fallback?).
             * Packet 3 (sequence number 3) is send from the communication address to server NATNEG3 (any kind of fallback?).
             * Each INIT packet is answered by an INIT_ACK packet as acknowledge to the original sender.
             * If server NATNEG1 have received all 4 INIT packets with sequence numbers 0 and 1 (same natneg-id), then it sends 2 CONNECT packets:
             * One packet is send to the communication address of the guest. The packet contains the public address of the host as data.
             * The other packet is send to the communication address of the host. The packet contains the public address of the quest as data.
             * Both clients send back a CONNECT_ACK packet to NATNEG1 as acknowledge.
             * Both clients start peer to peer communication using the public addresses.
             * 
             * C implementation:
             * See http://aluigi.altervista.org/papers/gsnatneg.c
             * 
             * Game names and game keys:
             * Civilization IV: Beyond the Sword             civ4bts         Cs2iIq
             * Mario Kart Wii (Wii)                          mariokartwii    9r3Rmy
             * 
             */
			try {
				IPEndPoint? remote = (IPEndPoint?)e.RemoteEndPoint;

				byte[] receivedBytes = new byte[e.BytesTransferred];
				Array.Copy(e.Buffer, e.Offset, receivedBytes, 0, e.BytesTransferred);

                NatNegMessage? message = null;
                try
                {
                    message = NatNegMessage.ParseData(receivedBytes);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError("[ServerNatNeg] - " + ex.ToString());
                }
                if (message == null)
                {
                    LoggerAccessor.LogWarn("[ServerNatNeg] - " + "Received unknown data " + string.Join(" ", receivedBytes.Select((b) => { return b.ToString("X2"); }).ToArray()) + " from " + remote.ToString() );
                }
                else
                {
                    LoggerAccessor.LogInfo("[ServerNatNeg] - " + "Received message " + message.ToString() + " from " + remote.ToString());
                    LoggerAccessor.LogInfo("[ServerNatNeg] - " + "(Message bytes: " + string.Join(" ", receivedBytes.Select((b) => { return b.ToString("X2"); }).ToArray()) + ")");
                    if (message.RecordType == 0)
                    {
                        // INIT, return INIT_ACK
                        message.RecordType = 1;
                        SendResponse(remote, message);

                        if (message.SequenceId > 1)
                        {
                            // Messages sent to natneg2 and natneg3, they only require an INIT_ACK. Used by client to determine NAT mapping mode?
                        }
                        else
                        {
                            // Collect data and send CONNECT messages if you have two peers initialized with all necessary data
                            if (!_Clients.ContainsKey(message.ClientId)) _Clients[message.ClientId] = new NatNegClient();
                            NatNegClient client = _Clients[message.ClientId];
                            client.ClientId = message.ClientId;
                            bool isHost = message.Hoststate > 0;
                            NatNegPeer peer = isHost ? client.Host : client.Guest;
                            if (peer == null) {
                                peer = new NatNegPeer();
                                if (isHost) client.Host = peer; else client.Guest = peer;
                            }
                            peer.IsHost = isHost;
                            if (message.SequenceId == 0)
                                peer.PublicAddress = remote;
                            else
                                peer.CommunicationAddress = remote;

                            if (client.Guest != null && client.Guest.CommunicationAddress != null && client.Guest.PublicAddress != null && client.Host != null && client.Host.CommunicationAddress != null && client.Host.PublicAddress != null) {
                                /* If server NATNEG1 have received all 4 INIT packets with sequence numbers 0 and 1 (same natneg-id), then it sends 2 CONNECT packets:
                                 * One packet is send to the communication address of the guest. The packet contains the public address of the host as data.
                                 * The other packet is send to the communication address of the host. The packet contains the public address of the quest as data.
                                 */

                                // Remove client from dictionary
                                NatNegClient? removed = null;
                                _Clients.TryRemove(client.ClientId, out removed);

                                message.RecordType = 5;
                                message.Error = 0;
                                message.GotData = 0x42;

                                message.ClientPublicIPAddress = NatNegMessage._toIpAddress(client.Host.PublicAddress.Address.GetAddressBytes());
                                message.ClientPublicPort = (ushort)client.Host.PublicAddress.Port;
                                SendResponse(client.Guest.CommunicationAddress, message);

                                message.ClientPublicIPAddress = NatNegMessage._toIpAddress(client.Guest.PublicAddress.Address.GetAddressBytes());
                                message.ClientPublicPort = (ushort)client.Guest.PublicAddress.Port;
                                SendResponse(client.Host.CommunicationAddress, message);

                                LoggerAccessor.LogInfo("[ServerNatNeg] - " + "Sent connect messages to peers with clientId " + client.ClientId + " connecting host " + client.Host.PublicAddress.ToString() + " and guest " + client.Guest.PublicAddress.ToString());
                            }
                        }
                    }
                    else if (message.RecordType == 13)
                    {
                        // REPORT, return REPORT_ACK
                        message.RecordType = 14;
                        SendResponse(remote, message);
                    }
                }
			} catch (Exception ex) {
                LoggerAccessor.LogError("[ServerNatNeg] - " + ex.ToString());
			}

			WaitForData();
		}

        private void SendResponse(IPEndPoint remote, NatNegMessage message)
        {
            byte[] response = message.ToBytes();
            LoggerAccessor.LogInfo("[ServerNatNeg] - " + "Sending response " + message.ToString() + " to " + remote.ToString());
            LoggerAccessor.LogInfo("[ServerNatNeg] - " + "(Response bytes: " + string.Join(" ", response.Select((b) => { return b.ToString("X2"); }).ToArray()) + ")");
            _socket?.SendTo(response, remote);
        }

    }
}
