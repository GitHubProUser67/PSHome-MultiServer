using CustomLogger;
using System.Net.Sockets;
using System.Net;
using EndianTools;

namespace EdenServer.AMHLair
{
    internal class TDUMasterServer
    {
        public Thread Thread;

        private static Socket? _socket;

        private readonly ManualResetEvent _reset = new ManualResetEvent(false);

        private EndPoint? senderEndPoint;

        public TDUMasterServer(IPAddress listen, ushort port)
        {
            Thread = new Thread(StartServer)
            {
                Name = "AMH MasterServer Retrieving Socket Thread"
            };
            Thread.Start(new AddressInfo()
            {
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
            try
            {
                if (disposing)
                {
                    if (_socket != null)
                    {
                        _socket.Close();
                        _socket.Dispose();
                        _socket = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        ~TDUMasterServer()
        {
            Dispose(false);
        }

        private void StartServer(object? parameter)
        {
            AddressInfo? info = (AddressInfo?)parameter;

            LoggerAccessor.LogInfo("[AMHMasterServer] - Starting Master Server");

            try
            {
                senderEndPoint = new IPEndPoint(info!.Address, info.Port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _socket.Bind(senderEndPoint);
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError("[AMHMasterServer] - " + String.Format("Unable to bind Master Server to {0}:{1}", info!.Address, info.Port));
                LoggerAccessor.LogError("[AMHMasterServer] - " + e.ToString());
                return;
            }

            while (true)
            {
                _reset.Reset();
                StartReceiving();
                _reset.WaitOne();
            }
        }

        private void StartReceiving()
        {
            try
            {
                byte[] buffer = new byte[8];

                _socket!.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref senderEndPoint!, new AsyncCallback(ReceiveCallback), new UdpState { Buffer = buffer, EndPoint = senderEndPoint });
            }
            catch (ObjectDisposedException)
            {

            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                _reset.Set();

                UdpState? state = (UdpState?)ar.AsyncState;
                if (state == null) return;

                LoggerAccessor.LogInfo("[AMHMasterServer] - " + String.Format("Received client request on EndPoint: {0}:{1}", ((IPEndPoint)state.EndPoint).Address, ((IPEndPoint)state.EndPoint).Port));

                _ = _socket!.EndReceiveFrom(ar, ref state.EndPoint);

                byte[] responseData = BuildServerResponse(state.Buffer);

                LoggerAccessor.LogInfo("[AMHMasterServer] - " + String.Format("Sent {0} byte response to: {1}:{2}", responseData.Length, ((IPEndPoint)state.EndPoint).Address, ((IPEndPoint)state.EndPoint).Port));

                _socket.SendTo(responseData, responseData.Length, SocketFlags.None, state.EndPoint);
            }
            catch (ObjectDisposedException)
            {

            }
            catch (Exception e)
            {
                LoggerAccessor.LogError("[AMHMasterServer] - Error sending data");
                LoggerAccessor.LogError("[AMHMasterServer] - " + e.ToString());
            }
        }

        private static byte[] BuildServerResponse(byte[] input)
        {
            /* Initial sequence is like this:
                - 0x1128 (ToMasterServer) 0x0000 (Padding) en (Region) 0x0000 (Padding)
                - 0x12345678 (uint enc key?) 0.0.0.0 (uint Target AMH Proxy IP)
            */

            byte[] response = new byte[8];

            if (input.Length == 8)
            {
                ushort crc = EndianAwareConverter.ToUInt16(input, Endianness.BigEndian, 6);

                switch (crc)
                {
                    case 0x1128: // Get Proxy Server params?

                        // TODO: use the region flag to point to regional proxies.
                        /*byte[] regionBytes = new byte[2];
                        Array.Copy(input, 2, regionBytes, 0, regionBytes.Length);*/

                        Array.Copy(IPAddress.Parse(EdenServerConfiguration.AMHProxyServerAddress).GetAddressBytes(), 0, response, 0, 4);

                        EndianAwareConverter.WriteUInt32(response, Endianness.BigEndian, 4, EdenServerConfiguration.AMHProxyEncryptionKey);

                        return response;
                    default:
                        LoggerAccessor.LogWarn($"[AMHMasterServer] - BuildServerResponse: unknown CRC:{crc:X4} requested. Falling back to default response...");
                        break;
                }
            }
            else
                LoggerAccessor.LogWarn($"[AMHMasterServer] - BuildServerResponse: unexpected input data received. Falling back to default response...");

            return response;
        }

        private class UdpState
        {
            public byte[] Buffer = new byte[8];
            public EndPoint? EndPoint;
        }
    }
}
