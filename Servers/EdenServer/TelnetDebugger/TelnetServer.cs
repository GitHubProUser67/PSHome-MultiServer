using CustomLogger;
using MultiServerLibrary.CustomServers;
using MultiServerLibrary.Extension;
using NetHasher.CRC;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace EdenServer.TelnetDebugger
{
    internal class TelnetServer
    {
        private static readonly Encoding _encoding = Encoding.UTF8;

        private readonly ConcurrentDictionary<uint, TcpClient> _clients = new();

        private TCPServer _server;

        public TelnetServer()
        {
            if (_server == null)
                _server = new TCPServer();
        }

        public void Start(ushort port)
        {
            _ = _server.StartAsync(
                new List<ushort> { port },
                1,
                null,
                null,
                null,
                (serverPort, client, remoteEP) =>
                {
                    uint clientId = CRC32.Create(Encoding.ASCII.GetBytes(serverPort.ToString() + $"MS4|{client.GetHashCode()}|{remoteEP}"));

                    if (_clients.TryAdd(clientId, client))
                    {
#if DEBUG
                        LoggerAccessor.LogInfo($"[TELNET] - Adding client with id:{clientId} to the cache.");
#endif
                        client.Client.Send(_encoding.GetBytes(" \r\nEdenServerDebugTelnet>")); // Send prompt.
                    }

                    const ushort telnetBuffSize = 1024;

                    byte[] buffer = new byte[telnetBuffSize];

                    using TcpClient storedClient = _clients[clientId];
                    using NetworkStream stream = storedClient.GetStream();

                    try
                    {
                        while (storedClient.IsConnected())
                        {
                            try
                            {
                                if (storedClient.Available > 0)
                                {
                                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                                    if (bytesRead == 0)
                                        // Client disconnected
                                        break;

                                    int startIndex = 0;
                                    // Skip Telnet negotiation bytes (255, etc.)
                                    while (startIndex < bytesRead && buffer[startIndex] == byte.MaxValue)
                                        startIndex += 3; // Telnet sequences are 3 bytes

                                    if (startIndex >= bytesRead)
                                        continue;

                                    LoggerAccessor.LogInfo($"[TELNET] - id:{clientId} sent Text:{{{_encoding.GetString(buffer, startIndex, bytesRead - startIndex)}}}");
                                }
                                else
                                    Thread.Sleep(1);
                            }
                            catch (Exception ex)
                            {
                                LoggerAccessor.LogError($"[TELNET] - Error reading from client: {ex}");
                                break;
                            }
                        }
                    }
                    catch
                    {
                        // Client disconnected
                    }

                    // Cleanup
                    if (_clients.TryRemove(clientId, out _))
                    {
#if DEBUG
                        LoggerAccessor.LogWarn($"[TELNET] - Removed client with id:{clientId} from the cache.");
#endif
                    }
                    else
                        LoggerAccessor.LogError($"[TELNET] - Failed to remove client with id:{clientId} from the cache. Should never happen!!!");
                },
                new CancellationTokenSource().Token
                );
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}
