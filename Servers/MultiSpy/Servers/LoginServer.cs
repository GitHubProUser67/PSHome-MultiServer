using CustomLogger;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiSpy.Servers
{
    internal class LoginServer
	{
		public Thread ThreadClientManager;
		public Thread ThreadSearchManager;

		private static Socket? _clientManagerSocket;
		private static Socket? _searchManagerSocket;

		private readonly ManualResetEvent _clientManagerReset = new ManualResetEvent(false);
		private readonly ManualResetEvent _searchManagerReset = new ManualResetEvent(false);

		public LoginServer(IPAddress listen, ushort clientManagerPort, ushort searchManagerPort)
		{
			ServicePointManager.SetTcpKeepAlive(true, 60 * 1000 * 10, 1000);

			ThreadClientManager = new Thread(StartServerClientManager) {
				Name = "Login Thread Client Manager"
			};
			ThreadClientManager.Start(new AddressInfo() {
				Address = listen,
				Port = clientManagerPort
			});

			ThreadSearchManager = new Thread(StartServerSearchManager) {
				Name = "Login Thread Search Manager"
			};
			ThreadSearchManager.Start(new AddressInfo() {
				Address = listen,
				Port = searchManagerPort
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
                if (_clientManagerSocket != null)
                {
                    try
                    {
                        _clientManagerSocket.Close();
                        _clientManagerSocket.Dispose();
                    }
                    catch { /* ignore */ }
                    _clientManagerSocket = null;
                }
                if (_searchManagerSocket != null)
                {
                    try
                    {
                        _searchManagerSocket.Close();
                        _searchManagerSocket.Dispose();
                    }
                    catch { /* ignore */ }
                    _searchManagerSocket = null;
                }
            }
		}

		~LoginServer()
		{
			Dispose(false);
		}

		private void StartServerClientManager(object? parameter)
		{
			AddressInfo? info = (AddressInfo?)parameter;

            LoggerAccessor.LogInfo("[LoginServer] - Starting Login Server ClientManager");

			try {
				_clientManagerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
					SendTimeout = 30000,
					ReceiveTimeout = 30000,
					SendBufferSize = 8192,
					ReceiveBufferSize = 8192,
					Blocking = false
				};

				_clientManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, true);
				_clientManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
				_clientManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
				_clientManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
				
				_clientManagerSocket.Bind(new IPEndPoint(info.Address, info.Port));
				_clientManagerSocket.Listen(10);
			} catch (Exception e) {
                LoggerAccessor.LogError("[LoginServer] - " + String.Format("Unable to bind Login Server ClientManager to {0}:{1}", info.Address, info.Port));
				LoggerAccessor.LogError("[LoginServer] - " + e.ToString());
				return;
			}

			while (true) {
				_clientManagerReset.Reset();

				LoginSocketState state = new LoginSocketState() {
					Type = LoginSocketState.SocketType.Client,
					Socket = _clientManagerSocket
				};

				_clientManagerSocket.BeginAccept(AcceptCallback, state);
				_clientManagerReset.WaitOne();
			}
		}

		private void StartServerSearchManager(object? parameter)
		{
			AddressInfo? info = (AddressInfo?)parameter;

            LoggerAccessor.LogInfo("[LoginServer] - " + "Starting Login Server SearchManager");

			try {
				_searchManagerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
					SendTimeout = 5000,
					ReceiveTimeout = 5000,
					SendBufferSize = 8192,
					ReceiveBufferSize = 8192,
					Blocking = false
				};

				_searchManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, true);
				_searchManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
				_searchManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, false);
				_searchManagerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

				_searchManagerSocket.Bind(new IPEndPoint(info.Address, info.Port));
				_searchManagerSocket.Listen(10);
			} catch (Exception e) {
                LoggerAccessor.LogError("[LoginServer] - " + String.Format("Unable to bind Login Server SearchManager to {0}:{1}", info.Address, info.Port));
                LoggerAccessor.LogError("[LoginServer] - " + e.ToString());
				return;
			}

			while (true) {
				_searchManagerReset.Reset();

				LoginSocketState state = new LoginSocketState() {
					Type = LoginSocketState.SocketType.Search,
					Socket = _searchManagerSocket
				};

				_searchManagerSocket.BeginAccept(AcceptCallback, state);
				_searchManagerReset.WaitOne();
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			LoginSocketState? state = (LoginSocketState?)ar.AsyncState;

			try {
				Socket client = state.Socket.EndAccept(ar);

				Thread.Sleep(1);

				if (state.Type == LoginSocketState.SocketType.Client)
					_clientManagerReset.Set();
				else if (state.Type == LoginSocketState.SocketType.Search)
					_searchManagerReset.Set();

				state.Socket = client;

                LoggerAccessor.LogInfo("[LoginServer] - " + String.Format("[{0}] New Client: {1}:{2}", state.Type, ((IPEndPoint)state.Socket.RemoteEndPoint).Address, ((IPEndPoint)state.Socket.RemoteEndPoint).Port));

				if (state.Type == LoginSocketState.SocketType.Client) {
					// ClientManager server sends data first
					SendToClient(ref state, LoginServerMessages.GenerateServerChallenge(ref state));

					if (state != null) {
						state.State++;
					}
				} else if (state.Type == LoginSocketState.SocketType.Search) {
					// SearchManager server waits for data first
				}
			} catch (NullReferenceException) {
				if (state != null)
					state.Dispose();
				state = null;
			} catch (SocketException e) {
                LoggerAccessor.LogError("[LoginServer] - Error accepting client");
                LoggerAccessor.LogError("[LoginServer] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
				if (state != null)
					state.Dispose();
				state = null;
				return;
			}
			catch (ObjectDisposedException)
			{

			}

			WaitForData(ref state);
		}

		public bool SendToClient(ref LoginSocketState? state, byte[] data)
		{
			if (data == null || state == null || state.Socket == null)
				return false;

			try {
				if (state.SendCallback == null)
					state.SendCallback = OnSent;

				state.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, state.SendCallback, state);
				return true;
			} catch (NullReferenceException) {
				if (state != null)
					state.Dispose();
				state = null;
				return false;
			} catch (SocketException e) {
				if (e.SocketErrorCode != SocketError.ConnectionAborted &&
					e.SocketErrorCode != SocketError.ConnectionReset) {
                    LoggerAccessor.LogError("[LoginServer] - Error sending data");
                    LoggerAccessor.LogError("[LoginServer] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
				}
				if (state != null)
					state.Dispose();
				state = null;
				return false;
			}
		}

		private void OnSent(IAsyncResult async)
		{
			LoginSocketState? state = (LoginSocketState?)async.AsyncState;

			if (state == null || state.Socket == null)
				return;

			try {
				int sent = state.Socket.EndSend(async);
                LoggerAccessor.LogInfo("[LoginServer] - " + String.Format("[{0}] Sent {1} byte response to: {2}:{3}", state.Type, sent, ((IPEndPoint)state.Socket.RemoteEndPoint).Address, ((IPEndPoint)state.Socket.RemoteEndPoint).Port));
			} catch (NullReferenceException) {
				if (state != null)
					state.Dispose();
				state = null;
			} catch (SocketException e) {
				switch (e.SocketErrorCode) {
					case SocketError.ConnectionReset:
					case SocketError.Disconnecting:
						if (state != null)
							state.Dispose();
						state = null;
						return;
					default:
                        LoggerAccessor.LogError("[LoginServer] - Error sending data");
                        LoggerAccessor.LogError("[LoginServer] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
						if (state != null)
							state.Dispose();
						state = null;
						return;
				}
			}
		}

		private void WaitForData(ref LoginSocketState? state)
		{
			Thread.Sleep(10);

			try {
				if (state.DataReceivedCallback == null)
					state.DataReceivedCallback = OnDataReceived;

				state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, state.DataReceivedCallback, state);
			} catch (NullReferenceException) {
				if (state != null)
					state.Dispose();
				state = null;
			} catch (ObjectDisposedException) {
				if (state != null)
					state.Dispose();
				state = null;
			} catch (SocketException e) {
				if (e.SocketErrorCode == SocketError.NotConnected) {
					if (state != null)
						state.Dispose();
					state = null;
					return;
				}

				if (e.SocketErrorCode != SocketError.ConnectionAborted &&
					e.SocketErrorCode != SocketError.ConnectionReset) {
                    LoggerAccessor.LogError("[LoginServer] - Error receiving data");
                    LoggerAccessor.LogError("[LoginServer] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
				}
				if (state != null)
					state.Dispose();
				state = null;
				return;
			}
		}

		private void OnDataReceived(IAsyncResult async)
		{
			LoginSocketState? state = (LoginSocketState?)async.AsyncState;

			if (state == null || state.Socket == null)
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

				// does what we received contain the \final\ delimiter?
				if (receivedData.LastIndexOf(@"\final\") > -1) {
					state.ReceivedData.Clear();

					// lets split up the message based on the delimiter
					string[] messages = receivedData.Split(new string[] { @"\final\" }, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < messages.Length; i++) {
						ParseMessage(ref state, messages[i]);
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
					case SocketError.Disconnecting:
					case SocketError.NotConnected:
					case SocketError.TimedOut:
						if (state != null)
							state.Dispose();
						state = null;
						return;
					default:
                        LoggerAccessor.LogError("[LoginServer] - Error receiving data");
                        LoggerAccessor.LogError("[LoginServer] - " + String.Format("{0} {1}", e.SocketErrorCode, e));
						if (state != null)
							state.Dispose();
						state = null;
						return;
				}
			} catch (Exception e) {
                LoggerAccessor.LogError("[LoginServer] - Error receiving data");
                LoggerAccessor.LogError("[LoginServer] - " + e.ToString());
			}

			// and we wait for more data...
			WaitForData(ref state);
		}

		private void ParseMessage(ref LoginSocketState state, string message)
		{
			string query;
			var keyValues = GetKeyValue(message, out query);

			if (keyValues == null || String.IsNullOrWhiteSpace(query)) {
				return;
			}

            LoggerAccessor.LogInfo("[LoginServer] - " + String.Format("[{0}] Received {1} query from: {2}:{3}", state.Type, query, ((IPEndPoint)state.Socket.RemoteEndPoint).Address, ((IPEndPoint)state.Socket.RemoteEndPoint).Port));

			switch (state.Type) {
				case LoginSocketState.SocketType.Client:
					HandleClientManager(ref state, query, keyValues);
					break;
				case LoginSocketState.SocketType.Search:
					HandleSearchManager(ref state, query, keyValues);
					break;
			}
		}
		
		private void HandleClientManager(ref LoginSocketState? state, string query, Dictionary<string, string> keyValues)
		{
			if (state == null || String.IsNullOrWhiteSpace(query) || keyValues == null) {
				return;
			}
#if DEBUG
			LoggerAccessor.LogInfo($"[LoginServer] - HandleClientManager - Client issued query:{query}.");
#endif
			if (state.State == 1) {
				if (query.Equals("login", StringComparison.InvariantCultureIgnoreCase))
				{
					SendToClient(ref state, LoginServerMessages.SendProof(ref state, keyValues));
					state.StartKeepAlive(this);
				}
				else if (query.Equals("newuser", StringComparison.InvariantCultureIgnoreCase))
					SendToClient(ref state, LoginServerMessages.NewUser(ref state, keyValues));
				else
					LoggerAccessor.LogError($"[LoginServer] - HandleClientManager - Unknown State 1 query:{query}, expect breakage!");
            }
            else if (state.State == 2)
			{
                if (query.Equals("logout", StringComparison.InvariantCultureIgnoreCase))
                    LoginServerMessages.Logout(ref state, keyValues);
                else if (query.Equals("getprofile", StringComparison.InvariantCultureIgnoreCase))
                    SendToClient(ref state, LoginServerMessages.SendProfile(ref state, keyValues, false));
                else if (query.Equals("updatepro", StringComparison.InvariantCultureIgnoreCase))
                    LoginServerMessages.UpdateProfile(ref state, keyValues);
                else if (query.Equals("status", StringComparison.InvariantCultureIgnoreCase))
                    LoginServerMessages.HandleStatus(ref state, keyValues);
                else
                    LoggerAccessor.LogError($"[LoginServer] - HandleClientManager - Unknown State 2 query:{query}, expect breakage!");
            }
            else if (state.State == 3)
			{
				if (query.Equals("logout", StringComparison.InvariantCultureIgnoreCase))
                    LoginServerMessages.Logout(ref state, keyValues);
                else if (query.Equals("getprofile", StringComparison.InvariantCultureIgnoreCase))
                    SendToClient(ref state, LoginServerMessages.SendProfile(ref state, keyValues, true));
                else if (query.Equals("status", StringComparison.InvariantCultureIgnoreCase))
                    LoginServerMessages.HandleStatus(ref state, keyValues);
                else
                    LoggerAccessor.LogError($"[LoginServer] - HandleClientManager - Unknown State 3 query:{query}, expect breakage!");
            }
            else if (state.State >= 4)
				state.Dispose();
        }

        private void HandleSearchManager(ref LoginSocketState? state, string query, Dictionary<string, string> keyValues)
		{
#if DEBUG
            LoggerAccessor.LogInfo($"[LoginServer] - HandleSearchManager - Search issued query:{query}.");
#endif
            if (state.State == 0) {
				if (query.Equals("nicks", StringComparison.InvariantCultureIgnoreCase))
                    SendToClient(ref state, LoginServerMessages.SendNicks(ref state, keyValues));
                else if (query.Equals("check", StringComparison.InvariantCultureIgnoreCase))
                    SendToClient(ref state, LoginServerMessages.SendCheck(ref state, keyValues));
                else
                    LoggerAccessor.LogError($"[LoginServer] - HandleSearchManager - Unknown State 0 query:{query}, expect breakage!");
            }
			else if (state.State == 1)
                state.State++;
            else if (state.State >= 2)
                state.Dispose();
        }

        private static Dictionary<string, string>? GetKeyValue(string message, out string query)
		{
			Dictionary<string, string> parsedData = new Dictionary<string, string>();

			string[] responseData = message.Split(new string[] { @"\" }, StringSplitOptions.None);

			if (responseData.Length > 1) {
				query = responseData[1];
			} else {
				query = String.Empty;
				return null;
			}

			for (int i = 1; i < responseData.Length - 1; i += 2) {
				if (parsedData.ContainsKey(responseData[i])) {
					parsedData[responseData[i].ToLowerInvariant()] = responseData[i + 1];
				} else {
					parsedData.Add(responseData[i].ToLowerInvariant(), responseData[i + 1]);
				}
			}

			return parsedData;
		}
	}

	internal class LoginSocketState : IDisposable
	{
		public enum SocketType
		{
			Client,
			Search
		}

		public AsyncCallback? SendCallback;
		public AsyncCallback? DataReceivedCallback;

		public SocketType Type;

		public Socket? Socket = null;
		public byte[] Buffer = new byte[8192];
		public StringBuilder ReceivedData = new StringBuilder(8192);

		public int State = 0;
		public int HeartbeatState = 0;
		public string Session = "";

		public string? ServerChallenge;
		public string? ClientChallenge;
		public string? Name;
		public string? Email;
		public string? PasswordEncrypted;

		private Timer? _keepAliveTimer;

		public void StartKeepAlive(LoginServer server)
		{
			if (_keepAliveTimer != null) {
				// if the timer already exists, destroy it so we can start a new one...
				_keepAliveTimer.Dispose();
			}

			// send a keep alive request every 2 minutes
			_keepAliveTimer = new Timer(KeepAliveCallback, server, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2));
		}
		
		private void KeepAliveCallback(object? s)
		{
			LoginServer? server = (LoginServer?)s;

			try {
				if (_keepAliveTimer == null) {
					Dispose();
					return;
				}

				LoginSocketState? state = this;
				HeartbeatState++;

				Console.WriteLine("sending keep alive");
				if (!server.SendToClient(ref state, LoginServerMessages.SendKeepAlive())) {
					Dispose();
					return;
				}

				// every 2nd keep alive request, we send an additional heartbeat
				if (HeartbeatState % 2 == 0) {
					Console.WriteLine("sending heartbeat");
					if (!server.SendToClient(ref state, LoginServerMessages.SendHeartbeat())) {
						Dispose();
						return;
					}
				}
			} catch (Exception e) {
				LoggerAccessor.LogError("[LoginServer] - Error running keep alive: " + e);
				Dispose();
			}
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
					SendCallback = null;
					DataReceivedCallback = null;

					if (Socket != null) {
						Socket.Shutdown(SocketShutdown.Both);
						Socket.Close();
						Socket.Dispose();
						Socket = null;
					}

					if (_keepAliveTimer != null) {
						_keepAliveTimer.Dispose();
						_keepAliveTimer = null;
					}
				}

				// yeah yeah, this is terrible, but it stops a memory leak :|
				GC.Collect();
			} catch {
			}
		}

		~LoginSocketState()
		{
			Dispose(false);
		}
	}
}
