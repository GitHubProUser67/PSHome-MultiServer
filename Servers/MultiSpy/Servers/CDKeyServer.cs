using CustomLogger;
using MultiSpyService.Utils;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiSpy.Servers
{
    internal class CDKeyServer
	{
		public Thread Thread;

		private const int BufferSize = 8192;
		private Socket? _socket;
		private SocketAsyncEventArgs? _socketReadEvent;
		private byte[]? _socketReceivedBuffer;

		private readonly Regex _dataPattern = new Regex(@"^\\auth\\\\pid\\1059\\ch\\[a-zA-z0-9]{8,10}\\resp\\(?<Challenge>[a-zA-z0-9]{72})\\ip\\\d+\\skey\\(?<Key>\d+)(\\reqproof\\[01]\\)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);
		private const string _dataResponse = @"\uok\\cd\{0}\skey\{1}";

		public CDKeyServer(IPAddress listen, ushort port)
		{
			Thread = new Thread(StartServer) {
				Name = "CD Key Thread"
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
			} catch {
			}
		}

		~CDKeyServer()
		{
			Dispose(false);
		}

		private void StartServer(object? parameter)
		{
			AddressInfo? info = (AddressInfo?)parameter;

			LoggerAccessor.LogInfo("[CDKeyServer] - Starting CD Key Server");

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
                LoggerAccessor.LogError("[CDKeyServer] - " + String.Format("Unable to bind CD Key Server to {0}:{1}", info.Address, info.Port));
                LoggerAccessor.LogError("[CDKeyServer] - " + e.ToString());
				return;
			}

			WaitForData();
		}

		private void WaitForData()
		{
			Thread.Sleep(10);

			try {
				_socket?.ReceiveFromAsync(_socketReadEvent);
			} catch (SocketException e) {
                LoggerAccessor.LogError("[CDKeyServer] - Error receiving data");
                LoggerAccessor.LogError("[CDKeyServer] - " + e.ToString());
				return;
			}
		}

		private void OnDataReceived(object? sender, SocketAsyncEventArgs e)
		{
			try {
				IPEndPoint? remote = (IPEndPoint?)e.RemoteEndPoint;

				string receivedData = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
				string decrypted = XorEncoding.Xor(receivedData);

				// known messages
				// \ka\ = keep alive from the game server every 20s, we don't care about this
				// \auth\ ... = authenticate cd key, this is what we care about
				// \disc\ ... = disconnect cd key, because there's checks if the cd key is in use, which we don't care about really, but we could if we wanted to

				// \ka\ is a keep alive from the game server, it's useless :p
				if (decrypted != @"\ka\") {
					Match m = _dataPattern.Match(decrypted);

					if (m.Success) {
                        LoggerAccessor.LogInfo("[CDKeyServer] - " + String.Format("Received request from: {0}:{1}", ((IPEndPoint)e.RemoteEndPoint).Address, ((IPEndPoint)e.RemoteEndPoint).Port));

						string reply = String.Format(_dataResponse, m.Groups["Challenge"].Value.Substring(0, 32), m.Groups["Key"].Value);

						byte[] response = Encoding.UTF8.GetBytes(XorEncoding.Xor(reply));
						_socket?.SendTo(response, remote);
					}
				}
			} catch (Exception) {
			}

			WaitForData();
		}
	}
}
