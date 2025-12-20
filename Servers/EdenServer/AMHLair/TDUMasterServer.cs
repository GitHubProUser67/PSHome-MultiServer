using CustomLogger;
using System.Net;
using EndianTools;
using MultiServerLibrary.CustomServers;

namespace EdenServer.AMHLair
{
    internal class TDUMasterServer
    {
        private UDPServer _server;

        public TDUMasterServer()
        {
            if (_server == null)
                _server = new UDPServer();
        }

        public void Start(ushort port)
        {
            _ = _server.StartAsync(
                new List<ushort> { port },
                Environment.ProcessorCount,
                null,
                null,
                null,
                (serverPort, listener, data, remoteEP) =>
                {
                    return BuildServerResponse(data);
                },
                new CancellationTokenSource().Token
                );
        }

        public void Stop()
        {
            _server.Stop();
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
    }
}
