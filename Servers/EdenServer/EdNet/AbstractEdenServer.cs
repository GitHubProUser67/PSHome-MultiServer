using System.Net;
using System.Net.Sockets;
using CustomLogger;
using EdenServer.EdNet.Messages;
using EdNetService.Models;
using MultiServerLibrary.CustomServers;
#if DEBUG
using CastleLibrary.Utils;
#endif


namespace EdenServer.EdNet
{
    public abstract class AbstractEdenServer
    {
        public abstract Dictionary<ushort, Type?> CrcToClass { get; }

        internal ClientStore ClientStore = new();

        private readonly UDPServer _server;

        public AbstractEdenServer()
        {
            _server ??= new UDPServer();
        }

        public void Start(ushort Port)
        {
            _ = _server.StartAsync(
                new List<ushort> { Port },
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
        protected virtual byte[]? ProcessMessagesFromClient(
            ushort serverPort,
            UdpClient listener,
            byte[] data,
            IPEndPoint remoteEP
        )
        {
            var receivedStore = new EdStore();

            receivedStore.LoadData(data, data.Length);
            var initialCrc = receivedStore.ExtractStart();
#if DEBUG
            LoggerAccessor.LogInfo(
                $"[EDEN_UDP] - {remoteEP.Address} Requested EdStore {initialCrc:X4} : {{{receivedStore.Data.BytesToHexStr().Replace("\n", string.Empty)}}}"
            );
#else
            LoggerAccessor.LogInfo(
                $"[EDEN_UDP] - {remoteEP.Address} Requested EdStore {initialCrc:X4}"
            );
#endif
            if (CrcToClass.TryGetValue(initialCrc, out var c))
            {
                AbstractMessage? msg = null;

                try
                {
                    if (c != null)
                        msg = (AbstractMessage?)Activator.CreateInstance(c);
                }
                catch { }

                msg?.Process(listener, this, remoteEP!, receivedStore);
            }
            else
                LoggerAccessor.LogError(
                    $"[EDEN_UDP] - {remoteEP.Address} Requested an unexpected message Type {initialCrc:X4} : SizeOfPacket:{data.Length}"
                );

            return null;
        }
        #endregion
    }
}
