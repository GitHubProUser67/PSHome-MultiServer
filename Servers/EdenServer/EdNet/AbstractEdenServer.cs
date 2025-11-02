using CustomLogger;
using EdenServer.EdNet.Messages;
using EdNetService.Models;
using MultiServerLibrary.CustomServers;
using MultiServerLibrary.Extension;
using System.Net;
using System.Net.Sockets;

namespace EdenServer.EdNet
{
    public abstract class AbstractEdenServer
    {
        public abstract Dictionary<ushort, Type?> CrcToClass { get; }

        internal ClientStore ClientStore = new ClientStore();

        private readonly UDPServer _server;

        public AbstractEdenServer()
        {
            if (_server == null)
                _server = new UDPServer();
        }

        public void Start(ushort Port)
        {
            _ = _server.StartAsync(
                new List<ushort> { Port },
                Environment.ProcessorCount,
                null,
                (serverPort, listener) =>
                {
                    ClientStore.Start();
                },
                null,
                ProcessMessagesFromClient,
                new CancellationTokenSource().Token
                );
        }

        public void Stop()
        {
            _server.Stop();
            ClientStore.Stop();
        }

        #region Protected Functions
        protected virtual byte[]? ProcessMessagesFromClient(ushort serverPort, UdpClient listener, byte[] data, IPEndPoint remoteEP)
        {
            EdStore receivedStore = new EdStore();

            receivedStore.LoadData(data, data.Length);
            ushort initialCrc = receivedStore.ExtractStart();
#if DEBUG
            LoggerAccessor.LogInfo($"[EDEN_UDP] - {remoteEP.Address} Requested EdStore {initialCrc:X4} : {{{receivedStore.Data.ToHexString().Replace("\n", string.Empty)}}}");
#else
            LoggerAccessor.LogInfo($"[EDEN_UDP] - {remoteEP.Address} Requested EdStore {initialCrc:X4}");
#endif
            if (CrcToClass.TryGetValue(initialCrc, out Type? c))
            {
                AbstractMessage? msg = null;

                try
                {
                    if (c != null)
                        msg = (AbstractMessage?)Activator.CreateInstance(c);
                }
                catch
                {
                }

                msg?.Process(listener, this, remoteEP!, receivedStore);
            }
            else
                LoggerAccessor.LogError($"[EDEN_UDP] - {remoteEP.Address} Requested an unexpected message Type {initialCrc:X4} : SizeOfPacket:{data.Length}");

            return null;
        }
        #endregion
    }
}
