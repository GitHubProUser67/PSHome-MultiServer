using CustomLogger;
using MultiSpyService.Data;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using MultiServerLibrary.Extension.LinqSQL;
using MultiSpyService.Utils;
using MultiSpyService.GSEncoding;

namespace MultiSpy.Servers
{
    internal class ServerListRetrieve
	{
		public Thread Thread;

		private static Socket? _socket;
		private readonly ServerListReport _report;

		private readonly ManualResetEvent _reset = new ManualResetEvent(false);
		private AsyncCallback? _socketSendCallback;
		private AsyncCallback? _socketDataReceivedCallback;

		public ServerListRetrieve(IPAddress listen, ushort port, ServerListReport report)
		{
			_report = report;
			/*
			_report.Servers.TryAdd("test", new List<GameServer>() {
				new GameServer() {
					Valid = true,
					IPAddress = "192.168.1.2",
					QueryPort = 29900,
					country = "AU",
					hostname = "[PR v1.2.0.0] 42",
					gamename = "battlefield2",
					gamever = "1.5.3153-802.0",
					mapname = "Awesome Map",
					gametype = "gpm_cq",
					gamevariant = "pr",
					numplayers = 100,
					maxplayers = 100,
					gamemode = "openplaying",
					password = false,
					timelimit = 14400,
					roundtime = 1,
					hostport = 16567,
					bf2_dedicated = true,
					bf2_ranked = true,
					bf2_anticheat = false,
					bf2_os = "win32",
					bf2_autorec = true,
					bf2_d_idx = "http://",
					bf2_d_dl = "http://",
					bf2_voip = true,
					bf2_autobalanced = false,
					bf2_friendlyfire = true,
					bf2_tkmode = "No Punish",
					bf2_startdelay = 240.0,
					bf2_spawntime = 300.0,
					bf2_sponsortext = "Welcome to an awesome server!",
					bf2_sponsorlogo_url = "http://",
					bf2_communitylogo_url = "http://",
					bf2_scorelimit = 100,
					bf2_ticketratio = 100.0,
					bf2_teamratio = 100.0,
					bf2_team1 = "US",
					bf2_team2 = "MEC",
					bf2_bots = false,
					bf2_pure = false,
					bf2_mapsize = 64,
					bf2_globalunlocks = true,
					bf2_fps = 35.0,
					bf2_plasma = true,
					bf2_reservedslots = 16,
					bf2_coopbotratio = 0,
					bf2_coopbotcount = 0,
					bf2_coopbotdiff = 0,
					bf2_novehicles = false
				}
			});

			IQueryable<GameServer> servers = _report.Servers.Select(x => x.Value).AsQueryable();
			Console.WriteLine(servers.Where("gamever = '1.5.3153-802.0' and gamevariant = 'pr' and hostname like '%[[]PR v1.2.0.0% %' and hostname like '%2%'").Count());
			*/

			Thread = new Thread(StartServer) {
				Name = "Server Retrieving Socket Thread"
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
			try {
				if (disposing) {
					if (_socket != null) {
						_socket.Close();
						_socket.Dispose();
						_socket = null;
					}
				}
			} catch (Exception) {
			}
		}

		~ServerListRetrieve()
		{
			Dispose(false);
		}

		private void StartServer(object? parameter)
		{
			AddressInfo? info = (AddressInfo?)parameter;

			LoggerAccessor.LogInfo("[ServerListRetreive] - Starting Server List Retrieval");

			try {
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
					SendTimeout = 5000,
					ReceiveTimeout = 5000,
					SendBufferSize = 65535,
					ReceiveBufferSize = 65535
				};
				_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, true);
				_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

				_socket.Bind(new IPEndPoint(info.Address, info.Port));
				_socket.Listen(10);
			} catch (Exception e) {
                LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("Unable to bind Server List Retrieval to {0}:{1}", info.Address, info.Port));
                LoggerAccessor.LogError("[ServerListRetreive] - " + e.ToString());
				return;
			}

			while (true) {
				_reset.Reset();
                _socket?.BeginAccept(AcceptCallback, _socket);
				_reset.WaitOne();
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			try
			{
                _reset.Set();

                Socket? listener = (Socket?)ar.AsyncState;
                Socket? handler = listener?.EndAccept(ar);

                SocketState state = new SocketState()
                {
                    Socket = handler
                };

                WaitForData(state);
            }
			catch (ObjectDisposedException)
			{

			}
		}

		private void WaitForData(SocketState state)
		{
			Thread.Sleep(10);
			if (state == null || state.Socket == null || !state.Socket.Connected)
				return;

			try {
				if (_socketDataReceivedCallback == null)
					_socketDataReceivedCallback = OnDataReceived;

				state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, _socketDataReceivedCallback, state);
			} catch (ObjectDisposedException) {
				state.Socket = null;
			} catch (SocketException e) {
				if (e.SocketErrorCode == SocketError.NotConnected)
					return;

                LoggerAccessor.LogError("[ServerListRetreive] - Error receiving data");
                LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
				return;
			}
		}

		private void OnDataReceived(IAsyncResult async)
		{
			SocketState? state = (SocketState?)async.AsyncState;

			if (state == null || state.Socket == null || !state.Socket.Connected)
				return;

			try {
				// receive data from the socket
				int received = state.Socket.EndReceive(async);
				if (received == 0) {
					// when EndReceive returns 0, it means the socket on the other end has been shut down.
					return;
				}

				// take what we received, and append it to the received data buffer
				state.ReceivedData.Append(Encoding.UTF8.GetString(state.Buffer, 0, received));
				string receivedData = state.ReceivedData.ToString();
				
				// does what we received end with \x00\x00\x00\x00\x??
				if (receivedData.Substring(receivedData.Length - 5, 4) == "\x00\x00\x00\x00") {
					state.ReceivedData.Clear();

					// lets split up the message based on the delimiter
					string[] messages = receivedData.Split(new string[] { "\x00\x00\x00\x00" }, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < messages.Length; i++) {
                        if (ParseRequest(state, messages[i]))
                            return;
                    }
				}
			} catch (ObjectDisposedException) {
				if (state != null)
					state.Dispose();
				state = null;
				return;
			} catch (SocketException e) {
				switch (e.SocketErrorCode) {
					case SocketError.ConnectionReset:
						if (state != null)
							state.Dispose();
						state = null;
						return;
					case SocketError.Disconnecting:
						if (state != null)
							state.Dispose();
						state = null;
						return;
					default:
                        LoggerAccessor.LogError("[ServerListRetreive] - Error receiving data");
                        LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
						if (state != null)
							state.Dispose();
						state = null;
						return;
				}
			} catch (Exception e) {
                LoggerAccessor.LogError("[ServerListRetreive] - Error receiving data");
                LoggerAccessor.LogError("[ServerListRetreive] - " + e.ToString());
			}

			// and we wait for more data...
			WaitForData(state);
		}

		private void SendToClient(SocketState state, byte[] data)
		{
			if (state == null)
				return;

			if (state.Socket == null || !state.Socket.Connected) {
				state.Dispose();
				state = null;
				return;
			}

			if (_socketSendCallback == null)
				_socketSendCallback = OnSent;

			try {
				state.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, _socketSendCallback, state);
			} catch (SocketException e) {
                LoggerAccessor.LogError("[ServerListRetreive] - Error sending data");
                LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
			}
		}

		private void OnSent(IAsyncResult async)
		{
			SocketState? state = (SocketState?)async.AsyncState;

			if (state == null || state.Socket == null)
				return;

			try {
				int sent = state.Socket.EndSend(async);
                LoggerAccessor.LogInfo("[ServerListRetreive] - " + String.Format("Sent {0} byte response to: {1}:{2}", sent, ((IPEndPoint)state.Socket.RemoteEndPoint).Address, ((IPEndPoint)state.Socket.RemoteEndPoint).Port));
			} catch (SocketException e) {
				switch (e.SocketErrorCode) {
					case SocketError.ConnectionReset:
					case SocketError.Disconnecting:
						return;
					default:
                        LoggerAccessor.LogError("[ServerListRetreive] - Error sending data");
                        LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
						return;
				}
			} finally {
				state.Dispose();
				state = null;
			}
		}

		private bool ParseRequest(SocketState state, string message)
		{
			string[] data = message.Split(new char[] { '\x00' }, StringSplitOptions.RemoveEmptyEntries);

			if (data.Length != 4)
                return false;

            string gamename = data[1].ToLowerInvariant();
			string validate = data[2].Substring(0, 8);
			string filter = data[2].Substring(8);
			string[] fields = data[3].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            LoggerAccessor.LogInfo("[ServerListRetreive] - " + String.Format("Received client request: {0}:{1}", ((IPEndPoint)state.Socket.RemoteEndPoint).Address, ((IPEndPoint)state.Socket.RemoteEndPoint).Port));

			IQueryable<GameServer> servers = _report.Servers.ToList().Select(x => x.Value).Where(x => x.Valid).AsQueryable();

			string fixedFilter = FixFilter(filter);
			if (!String.IsNullOrWhiteSpace(fixedFilter))
			{
				try {
					//Console.WriteLine(filter);
					servers = servers.Where(fixedFilter);
					//Console.WriteLine(servers.Count());
				} catch (Exception e) {
                    LoggerAccessor.LogError("[ServerListRetreive] - Error parsing filter");
                    LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("  Source filter: {0}", filter));
                    LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("  Fixed filter:  {0}", fixedFilter));
                    LoggerAccessor.LogError("[ServerListRetreive] - " + String.Format("  {0}", e.ToString()));

					// Return empty server list
					servers = (new List<GameServer>()).AsQueryable();
				}
			}

			byte[] unencryptedServerList = PackServerList(state, servers, fields);
			byte[] encryptedServerList = EncodingHelper.Encode(DataFunctions.StringToBytes(MultiSpyServerConfiguration.GetKeyFromGameName(gamename)),
				DataFunctions.StringToBytes(validate), unencryptedServerList, unencryptedServerList.LongLength);
			SendToClient(state, encryptedServerList);
			return true;
		}

		private static byte[] PackServerList(SocketState state, IEnumerable<GameServer> servers, string[] fields)
		{
			IPEndPoint? remoteEndPoint = ((IPEndPoint?)state.Socket.RemoteEndPoint);

			byte[] ipBytes = remoteEndPoint.Address.GetAddressBytes();
			byte[] value2 = BitConverter.GetBytes((ushort)6500);
			byte fieldsCount = (byte)fields.Length;

			List<byte> data = new List<byte>();
			data.AddRange(ipBytes);
			data.AddRange(BitConverter.IsLittleEndian ? value2.Reverse() : value2);
			data.Add(fieldsCount);
			data.Add(0);

			foreach (var field in fields) {
				data.AddRange(DataFunctions.StringToBytes(field));
				data.AddRange(new byte[] { 0, 0 });
			}

			foreach (var server in servers) {
				// commented this stuff out since it caused some issues on testing, might come back to it later and see what's happening...
				// NAT traversal stuff...
				// 126 (\x7E)	= public ip / public port / private ip / private port / icmp ip
				// 115 (\x73)	= public ip / public port / private ip / private port
				// 85 (\x55)	= public ip / public port
				// 81 (\x51)	= public ip / public port
				/*Console.WriteLine(server.IPAddress);
				Console.WriteLine(server.QueryPort);
				Console.WriteLine(server.localip0);
				Console.WriteLine(server.localip1);
				Console.WriteLine(server.localport);
				Console.WriteLine(server.natneg);
				if (!String.IsNullOrWhiteSpace(server.localip0) && !String.IsNullOrWhiteSpace(server.localip1) && server.localport > 0) {
					data.Add(126);
					data.AddRange(IPAddress.Parse(server.IPAddress).GetAddressBytes());
					data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.QueryPort).Reverse() : BitConverter.GetBytes((ushort)server.QueryPort));
					data.AddRange(IPAddress.Parse(server.localip0).GetAddressBytes());
					data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.localport).Reverse() : BitConverter.GetBytes((ushort)server.localport));
					data.AddRange(IPAddress.Parse(server.localip1).GetAddressBytes());
				} else if (!String.IsNullOrWhiteSpace(server.localip0) && server.localport > 0) {
					data.Add(115);
					data.AddRange(IPAddress.Parse(server.IPAddress).GetAddressBytes());
					data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.QueryPort).Reverse() : BitConverter.GetBytes((ushort)server.QueryPort));
					data.AddRange(IPAddress.Parse(server.localip0).GetAddressBytes());
					data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.localport).Reverse() : BitConverter.GetBytes((ushort)server.localport));
				} else {*/
					data.Add(81); // it could be 85 as well, unsure of the difference, but 81 seems more common...
					data.AddRange(IPAddress.Parse(server.IPAddress).GetAddressBytes());
					data.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes((ushort)server.QueryPort).Reverse() : BitConverter.GetBytes((ushort)server.QueryPort));
				//}

				data.Add(255);

				for (int i = 0; i < fields.Length; i++) {
                    if (server.GetType().GetProperty(fields[i]) == null)
                    {
                       LoggerAccessor.LogWarn(string.Format("[ServerListRetreive] - Unknown server property '{0}' requested in server list", fields[i]));
                        data.AddRange(DataFunctions.StringToBytes(string.Empty));
                    }
                    else
                        data.AddRange(DataFunctions.StringToBytes(GetField(server, fields[i])));

					if (i < fields.Length - 1)
						data.AddRange(new byte[] { 0, 255 });
				}

				data.Add(0);
			}

			data.AddRange(new byte[] { 0, 255, 255, 255, 255 });

			return data.ToArray();
		}

		private static string? GetField(GameServer server, string fieldName)
		{
			object? value = server.GetType().GetProperty(fieldName)?.GetValue(server, null);
			if (value == null)
				return String.Empty;
			else if (value is Boolean)
				return (bool)value ? "1" : "0";
			else
				return value.ToString();
		}

		private enum FilterWordTypes
		{
			None,
			String,
			OpenBracket,
			CloseBracket,
			Comparison,
			Logical,
			Other,
		};

		private static string FixFilter(string filter)
		{
			// get all the properties that aren't "[NonFilter]"
			PropertyInfo[] properties = typeof(GameServer).GetProperties();
			List<string> filterableProperties = new List<string>();
			foreach (var property in properties)
			{
				if (property.GetCustomAttributes(false).Any(x => x.GetType().Name == "NonFilterAttribute"))
					continue;

				filterableProperties.Add(property.Name);
			}
			
			// escape [
			filter = filter.Replace("[", "[[]");

			StringBuilder filterBuilder = new StringBuilder();
			int len = filter.Length;
			var prevWordTrueType = FilterWordTypes.None;
			var curWordType = FilterWordTypes.None;
			int curWordStart = 0;
			int endOfString = -1;
			for(int i = 0; i < len; i++)
			{
				FilterWordTypes newWordType;
				if (i <= endOfString) {
					newWordType = FilterWordTypes.String;
				} else {
					char ch = filter[i];
					if (ch == '\'' || ch == '"')
					{
						newWordType = FilterWordTypes.String;

						// Search for the trailing quote
						// This is a nightmare, since they forgot to escape filter strings in the BF2 client, so you can easily get something like that:
						//	  hostname like 'flyin' high'
						int quotes = filter.Substring(i + 1).Count(x => x == ch);
						if (quotes == 0)
							endOfString = len - 1; // No trailing quote
						else if (quotes == 1)
							endOfString = filter.IndexOf(ch, i + 1);
						else // quotes > 1
						{
							endOfString = i;
							bool doPercentCheck = (filter[i + 1] == '%');
							for (int j = 1; j <= quotes; j++)
							{
								endOfString = filter.IndexOf(ch, endOfString + 1);
								if (j == quotes) // Last quote?
									break;

								if (doPercentCheck)
								{
									if (endOfString <= (i + 2))
										continue;
									if (filter[endOfString - 1] != '%')
										continue;
								}

								string trailStr = filter.Substring(endOfString + 1).TrimStart();
								bool isTerminated = (trailStr.StartsWith(")") 
														|| trailStr.StartsWith("(")
														|| trailStr.StartsWith("and ", StringComparison.InvariantCultureIgnoreCase)
														|| trailStr.StartsWith("or ", StringComparison.InvariantCultureIgnoreCase));
								if (isTerminated == false) {
									foreach(var property in filterableProperties)
										if (trailStr.StartsWith(property)) {
											isTerminated = true;
											break;
										}
								}

								if (isTerminated)
									break;
							}
						}
					}
					else if (ch <= ' ')
						newWordType = FilterWordTypes.None; // Skip whitespaces
					else if (ch == '(')
						newWordType = FilterWordTypes.OpenBracket;
					else if (ch == ')')
						newWordType = FilterWordTypes.CloseBracket;
					else if (ch == '=' || ch ==  '!' || ch == '<' || ch == '>')
						newWordType = FilterWordTypes.Comparison;
					//else if (ch == '&' || ch == '|') // No idea how these C logical operators can get into a BF2 filter, but they were in the original...
					//	newWordType = FilterWordTypes.Logical;
					else
						newWordType = FilterWordTypes.Other;
				}

				if (newWordType != curWordType || newWordType == FilterWordTypes.OpenBracket || newWordType == FilterWordTypes.CloseBracket)
				{
					if (curWordType != FilterWordTypes.None)
					{
						prevWordTrueType = AddFilterWord(filterBuilder, filter, curWordStart, i, curWordType, prevWordTrueType, filterableProperties);
					}

					curWordType = newWordType;
					curWordStart = i;
				}
			}

			if (curWordType != FilterWordTypes.None && curWordStart < len)
			{
				AddFilterWord(filterBuilder, filter, curWordStart, len, curWordType, prevWordTrueType, filterableProperties);
			}

			return filterBuilder.ToString();
		}

		private static FilterWordTypes AddFilterWord(StringBuilder filterBuilder, string filter, int wordStart, int nextWordStart, FilterWordTypes wordType, FilterWordTypes prevWordType, List<string> filterableProperties)
		{
			string word = filter.Substring(wordStart, nextWordStart - wordStart);

			if (wordType == FilterWordTypes.Other)
			{
				// Try to fix properties merged with other stuff
				foreach(var property in filterableProperties)
				{
					int propIndex = word.IndexOf(property);
					if (propIndex < 0)
						continue;

					if (propIndex > 0)
						prevWordType = AddFilterWord(filterBuilder, word.Substring(0, propIndex), FilterWordTypes.Other, prevWordType);

					prevWordType = AddFilterWord(filterBuilder, property, FilterWordTypes.Other, prevWordType);

					int trailIndex = propIndex + property.Length;
					if (trailIndex < word.Length)
						prevWordType = AddFilterWord(filterBuilder, word.Substring(trailIndex), FilterWordTypes.Other, prevWordType);

					return prevWordType;
				}
			}

			return AddFilterWord(filterBuilder, word, wordType, prevWordType);
		}

		private static FilterWordTypes AddFilterWord(StringBuilder filterBuilder, string word, FilterWordTypes wordType, FilterWordTypes prevWordType)
		{
			if (wordType == FilterWordTypes.Other)
			{
				if (word.Equals("and", StringComparison.InvariantCultureIgnoreCase))
					wordType = FilterWordTypes.Logical;
				else if (word.Equals("or", StringComparison.InvariantCultureIgnoreCase))
					wordType = FilterWordTypes.Logical;
				else if (word.Equals("like", StringComparison.InvariantCultureIgnoreCase))
					wordType = FilterWordTypes.Comparison;
				else if (word.Equals("not", StringComparison.InvariantCultureIgnoreCase))
					wordType = FilterWordTypes.Comparison;
			}

			// Not the first word or start/end of a group
			if (prevWordType != FilterWordTypes.None && prevWordType != FilterWordTypes.OpenBracket && wordType != FilterWordTypes.CloseBracket)
			{
				filterBuilder.Append(' ');

				// fix an issue in the BF2 main menu where filter expressions aren't joined properly
				// i.e. "numplayers > 0gametype like '%gpm_cq%'"
				// becomes "numplayers > 0 and gametype like '%gpm_cq%'"
				if (wordType == FilterWordTypes.Other)
				{
					if (prevWordType != FilterWordTypes.Logical && prevWordType != FilterWordTypes.Comparison)
						filterBuilder.Append("and ");
				} else if (wordType == FilterWordTypes.OpenBracket) {
					if (prevWordType == FilterWordTypes.Other || prevWordType == FilterWordTypes.String)
						filterBuilder.Append("and ");
				}
			}

			if (wordType == FilterWordTypes.String)
			{
				char quote = word[0];
				filterBuilder.Append(quote);
				if (word.Length > 2)
				{
					string strContent = word.Substring(1, word.Length - 2);
					filterBuilder.Append(strContent.Replace(quote, '_')); // replace quote characters inside the string with a wildcard character
				}
				filterBuilder.Append(quote);
			}
			else
				filterBuilder.Append(word);

			return wordType;
		}

		private class SocketState : IDisposable
		{
			public Socket? Socket = null;
			public byte[] Buffer = new byte[8192];
			public StringBuilder ReceivedData = new StringBuilder(8192);

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				try {
					if (disposing) {
						if (Socket != null) {
							try {
								Socket.Shutdown(SocketShutdown.Both);
							} catch (Exception) {
							}
							Socket.Close();
							Socket.Dispose();
							Socket = null;
						}
					}

					// yeah yeah, this is terrible, but it stops a memory leak :|
					GC.Collect();
				} catch {
				}
			}

			~SocketState()
			{
				Dispose(false);
			}
		}
	}
}
