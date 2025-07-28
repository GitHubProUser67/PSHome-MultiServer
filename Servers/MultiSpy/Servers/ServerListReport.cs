using CustomLogger;
using EndianTools;
using MultiServerLibrary.Extension.LinqSQL;
using MultiServerLibrary.GeoLocalization;
using MultiSpyService.Data;
using MultiSpyService.Utils;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiSpy.Servers
{
	internal enum ServerListMasterMsg
	{
        CHALLENGE_RESPONSE = 0x01,
	    HEARTBEAT = 0x03,
	    KEEPALIVE = 0x08,
	    AVAILABLE = 0x09,
	    RESPONSE_CORRECT = 0x0A
    }

    internal class ServerListReport
	{
		public readonly ConcurrentDictionary<string, GameServer> Servers;

		private string[]? ModWhitelist;
		private IPAddress[]? PlasmaServers;

		public Thread Thread;

		private const int BufferSize = 65535;
		private Socket? _socket;
		private SocketAsyncEventArgs? _socketReadEvent;
		private byte[]? _socketReceivedBuffer;

		public ServerListReport(IPAddress listen, ushort port)
		{
			Servers = new ConcurrentDictionary<string, GameServer>();

			Thread = new Thread(StartServer) {
				Name = "Server Reporting Socket Thread"
			};
			Thread.Start(new AddressInfo() {
				Address = listen,
				Port = port
			});

			new Thread(StartCleanup) {
				Name = "Server Reporting Cleanup Thread"
			}.Start();

			new Thread(StartDynamicInfoReload) {
				Name = "Dynamic Info Reload Thread"
			}.Start();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					if (_socket != null) {
						_socket.Close();
						_socket.Dispose();
						_socket = null;
					}
				}
			} catch {
			}
		}

		~ServerListReport()
		{
			Dispose(false);
		}

		private void StartServer(object? parameter)
		{
			AddressInfo? info = (AddressInfo?)parameter;

			LoggerAccessor.LogInfo("[ServerListReport] - Starting Server List Reporting");

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
                LoggerAccessor.LogError("[ServerListReport] - " + String.Format("Unable to bind Server List Reporting to {0}:{1}", info.Address, info.Port));
                LoggerAccessor.LogError("[ServerListReport] - " + e.ToString());
				return;
			}

			WaitForData();
		}

		private void StartCleanup(object? parameter)
		{
			while (true) {
				foreach (var key in Servers.Keys) {
					GameServer? value;

					if (Servers.TryGetValue(key, out value)) {
						if (value.LastPing < DateTime.UtcNow - TimeSpan.FromSeconds(30)) {
                            LoggerAccessor.LogWarn("[ServerListReport] - " + String.Format("Removing old server at: {0}", key));

							GameServer? temp;
							Servers.TryRemove(key, out temp);
						}
					}
				}

				Thread.Sleep(10000);
			}
		}

		private void StartDynamicInfoReload(object? obj)
		{
			while (true) {
				// the modwhitelist.txt file is for only allowing servers running certain mods to register with the master server
				// by default, this is pr or pr_* (it's really pr!_%, since % is wildcard, _ is placeholder, ! is escape)
				// # is for comments
				// you either want to utilize modwhitelist.txt or hardcode the default if you're using another mod...
				// put each mod name on a new line
				// to allow all mods, just put a single %
				if (File.Exists("modwhitelist.txt")) {
                    LoggerAccessor.LogInfo("[ServerListReport] - Loading mod whitelist");
					ModWhitelist = File.ReadAllLines("modwhitelist.txt").Where(x => !String.IsNullOrWhiteSpace(x) && !x.Trim().StartsWith("#")).ToArray();
				} else {
					ModWhitelist = new string[] { "pr", "pr!_%" };
				}

				// plasma servers (bf2_plasma = 1) makes servers show up in green in the server list in bf2's main menu (or blue in pr's menu)
				// this could be useful to promote servers and make them stand out, sponsored servers, special events, stuff like that
				// put in the ip address of each server on a new line in plasmaservers.txt, and make them stand out
				if (File.Exists("plasmaservers.txt")) {
                    LoggerAccessor.LogInfo("[ServerListReport] - Loading plasma servers");
					PlasmaServers = File.ReadAllLines("plasmaservers.txt").Select(x => {
						IPAddress address;
						if (IPAddress.TryParse(x, out address))
							return address;
						else
							return null;
					}).Where(x => x != null).ToArray();
				} else {
					PlasmaServers = new IPAddress[0];
				}

				GC.Collect();

				Thread.Sleep(5 * 60 * 1000);
			}
		}

		private void WaitForData()
		{
			Thread.Sleep(10);
			GC.Collect();

			try {
				_socket?.ReceiveFromAsync(_socketReadEvent);
			} catch (SocketException e) {
                LoggerAccessor.LogError("[ServerListReport] - " + "Error receiving data");
                LoggerAccessor.LogError("[ServerListReport] - " + e.ToString());
				return;
			}
		}

		private void OnDataReceived(object? sender, SocketAsyncEventArgs e)
		{
			try {
				IPEndPoint? remote = (IPEndPoint?)e.RemoteEndPoint;

				byte[] receivedBytes = new byte[e.BytesTransferred];
				Array.Copy(e.Buffer, e.Offset, receivedBytes, 0, e.BytesTransferred);

                // there by a bunch of different message formats...

                if (receivedBytes.Length == 5 && receivedBytes[0] == (byte)ServerListMasterMsg.KEEPALIVE)
                {
                    // this is a server ping, it starts with 0x08, it happens every 20 seconds or so

                    byte[] uniqueId = new byte[4];
                    Array.Copy(receivedBytes, 1, uniqueId, 0, 4);

                    RefreshServerPing(remote);
                }
                else if (receivedBytes.Length >= 5 && receivedBytes[0] == (byte)ServerListMasterMsg.AVAILABLE)
                {
                    // the initial message is basically the gamename, 0x09 0x00 0x00 0x00 0x00 battlefield2
                    // reply back a good response

                    _socket.SendTo(new byte[] { 0xfe, 0xfd, 0x09, 0x00, 0x00, 0x00, 0x00 }, remote); // AVAILABLE_RESPONSE
                    // _socket.SendTo(new byte[] { 0xfe, 0xfd, 0x09, 0x00, 0x00, 0x00, 0x01 }, remote); // UNAVAILABLE_RESPONSE
                    // _socket.SendTo(new byte[] { 0xfe, 0xfd, 0x09, 0x00, 0x00, 0x00, 0x02 }, remote); // TEMP_UNAVAILABLE_RESPONSE
                }
                else if (receivedBytes.Length > 5 && receivedBytes[0] == (byte)ServerListMasterMsg.HEARTBEAT)
                {
                    // this is where server details come in, it starts with 0x03, it happens every 60 seconds or so

                    byte[] uniqueId = new byte[4];
                    Array.Copy(receivedBytes, 1, uniqueId, 0, 4);

                    if (!ParseServerDetails(remote, receivedBytes.Skip(5).ToArray()))
                    {
						List<byte> response = new List<byte>();

						response.AddRange(new byte[] { 0xfe, 0xfd, (byte)ServerListMasterMsg.CHALLENGE_RESPONSE, uniqueId[0], uniqueId[1], uniqueId[2], uniqueId[3] });

                        response.AddRange(Encoding.ASCII.GetBytes(new HeartbeatCipher().Salt));

                        byte[] ipBytes = remote.Address.GetAddressBytes(); // 4 bytes
                        byte[] portBytes = new byte[2]; // 2 bytes, big-endian

						EndianAwareConverter.WriteUInt16(portBytes, Endianness.BigEndian, 0, (ushort)remote.Port);

                        var ipPortBlock = new byte[1 + ipBytes.Length + portBytes.Length];
                        ipPortBlock[0] = 0x00;
                        Array.Copy(ipBytes, 0, ipPortBlock, 1, ipBytes.Length);
                        Array.Copy(portBytes, 0, ipPortBlock, 1 + ipBytes.Length, portBytes.Length);

                        // 6. Base16 encode the block (uppercase ASCII hex)
                        response.AddRange(Encoding.ASCII.GetBytes(BitConverter.ToString(ipPortBlock).Replace("-", string.Empty).ToUpper()));

                        response.Add(0x00);

                        _socket.SendTo(response.ToArray(), remote);
                    }
                }
                else if (receivedBytes.Length > 5 && receivedBytes[0] == (byte)ServerListMasterMsg.CHALLENGE_RESPONSE)
                {
                    // this is a challenge response, it starts with 0x01

                    byte[] uniqueId = new byte[4];
                    Array.Copy(receivedBytes, 1, uniqueId, 0, 4);

                    // confirm against the challenge (UNIMPLEMENTED)
                    byte[] clientResponse = new byte[29];
                    Array.Copy(receivedBytes, 5, clientResponse, 0, clientResponse.Length);

                    // Reply back a good response all the time for now
                    byte[] response = new byte[] { 0xfe, 0xfd, (byte)ServerListMasterMsg.RESPONSE_CORRECT, uniqueId[0], uniqueId[1], uniqueId[2], uniqueId[3] };
                    _socket.SendTo(response, remote);

                    AddValidServer(remote);
                }
            } catch (Exception ex) {
                LoggerAccessor.LogError("[ServerListReport] - " + ex.ToString());
			}

			WaitForData();
		}

		private void RefreshServerPing(IPEndPoint remote)
		{
			string key = String.Format("{0}:{1}", remote.Address, remote.Port);
			if (Servers.ContainsKey(key)) {
				GameServer value;
				if (Servers.TryGetValue(key, out value)) {
					value.LastPing = DateTime.UtcNow;
					Servers[key] = value;
				}
			}
		}

		private bool ParseServerDetails(IPEndPoint remote, byte[] data)
		{
			string key = string.Format("{0}:{1}", remote.Address, remote.Port);
			string receivedData = Encoding.UTF8.GetString(data);
			
			// split by 000 (info/player separator) and 002 (players/teams separator)
			// the players/teams separator is really 00, but because 00 may also be used elsewhere (an empty value for example), we hardcode it to 002
			// the 2 is the size of the teams, for BF2 this is always 2.
			string[] sections = receivedData.Split(new string[] { "\x00\x00\x00", "\x00\x00\x02" }, StringSplitOptions.None);
			
			if (sections.Length != 3 && !receivedData.EndsWith("\x00\x00"))
				return true; // true means we don't send back a response

			string serverVars = sections[0];
			//string playerVars = sections[1];
			//string teamVars = sections[2];

			string[] serverVarsSplit = serverVars.Split(new string[] { "\x00" }, StringSplitOptions.None);

			GameServer server = new GameServer() {
				Valid = false,
				IPAddress = remote.Address.ToString(),
				QueryPort = remote.Port,
				LastRefreshed = DateTime.UtcNow,
				LastPing = DateTime.UtcNow
			};

			// set the country based off ip address
			if (GeoIP.Instance == null || GeoIP.Instance.Reader == null) {
				server.country = "??";
			} else {
				try {
                    var isoCode = GeoIP.GetISOCodeFromIP(IPAddress.Parse(server.IPAddress));
                    server.country = string.IsNullOrEmpty(isoCode) ? "??" : isoCode.ToUpperInvariant();
				} catch (Exception e) {
                    LoggerAccessor.LogError("[ServerListReport] - " + e.ToString());
					server.country = "??";
				}
			}

			for (int i = 0; i < serverVarsSplit.Length - 1; i += 2) {
				PropertyInfo? property = server.GetType().GetProperty(serverVarsSplit[i]);

				if (property == null)
					continue;
#if DEBUG
				LoggerAccessor.LogInfo($"[ServerListReport] - property with name:{property.Name} was requested.");
#endif
				if (property.Name == "hostname") {
					// strip consecutive whitespace from hostname
					property.SetValue(server, Regex.Replace(serverVarsSplit[i + 1], @"\s+", " ").Trim(), null);
				} else if (property.Name == "bf2_plasma") {
					// set plasma to true if the ip is in plasmaservers.txt
					if (PlasmaServers.Any(x => x.Equals(remote.Address)))
						property.SetValue(server, true, null);
					else
						property.SetValue(server, false, null);
				} else if (property.Name == "bf2_ranked") {
					// we're always a ranked server (helps for mods with a default bf2 main menu, and default filters wanting ranked servers)
					property.SetValue(server, true, null);
				} else if (property.Name == "bf2_pure") {
					// we're always a pure server
					property.SetValue(server, true, null);
				} else if (property.PropertyType == typeof(Boolean)) {
					// parse string to bool (values come in as 1 or 0)
					int value;
					if (Int32.TryParse(serverVarsSplit[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
						property.SetValue(server, value != 0, null);
					}
				} else if (property.PropertyType == typeof(Int32)) {
					// parse string to int
					int value;
					if (Int32.TryParse(serverVarsSplit[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
						property.SetValue(server, value, null);
					}
				} else if (property.PropertyType == typeof(Double)) {
					// parse string to double
					double value;
					if (Double.TryParse(serverVarsSplit[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
						property.SetValue(server, value, null);
					}
				} else if (property.PropertyType == typeof(String)) {
					// parse string to string
					property.SetValue(server, serverVarsSplit[i + 1], null);
				}
			}

			if (!String.IsNullOrWhiteSpace(server.gamevariant) && !ModWhitelist.ToList().Any(x => SQLMethods.EvaluateIsLike(server.gamevariant, x))) {
				// only allow servers with a gamevariant of those listed in modwhitelist.txt, or (pr || pr_*) by default
				return true; // true means we don't send back a response
			}

			// you've got to have all these properties in order for your server to be valid
			if (!String.IsNullOrWhiteSpace(server.hostname) &&
				server.hostport > 1024 && server.hostport <= UInt16.MaxValue &&
				server.maxplayers > 0) {
				server.Valid = true;
			}

			// if the server list doesn't contain this server, we need to return false in order to send a challenge
			// if the server replies back with the good challenge, it'll be added in AddValidServer
			if (!Servers.ContainsKey(key))
				return false;

			Servers.AddOrUpdate(key, server, (k, old) => {
				if (!old.Valid && server.Valid) {
                    LoggerAccessor.LogInfo("[ServerListReport] - " + String.Format("Added new server at: {0}:{1} ({2}) ({3})", server.IPAddress, server.QueryPort, server.country, server.gamevariant));
				}

				return server;
			});

			return true;
		}

		private void AddValidServer(IPEndPoint remote)
		{
			string key = String.Format("{0}:{1}", remote.Address, remote.Port);
			GameServer server = new GameServer() {
				Valid = false,
				IPAddress = remote.Address.ToString(),
				QueryPort = remote.Port,
				LastRefreshed = DateTime.UtcNow,
				LastPing = DateTime.UtcNow
			};

			Servers.AddOrUpdate(key, server, (k, old) => {
				return server;
			});
		}
	}
}
