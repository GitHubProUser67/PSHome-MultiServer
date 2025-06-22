using CustomLogger;
using EdenServer.EdNet.Messages;
using EdNetService.Models;
using NetworkLibrary.Extension;
using System.Net;
using System.Net.Sockets;

namespace EdenServer.EdNet
{
    public abstract class AbstractEdenServer
    {
        public static bool IsStarted = false;
        public abstract Dictionary<ushort, Type?> CrcToClass { get; }

        private Thread? thread;
        private volatile bool threadActive;

        internal UdpClient? listener;

        private List<Task> UdpTasks = new();

        internal ClientStore ClientStore = new ClientStore();

        private readonly ushort Port;
        private readonly int AwaiterTimeoutInMS;
        private readonly int MaxConcurrentListeners;

        public AbstractEdenServer(ushort Port, int MaxConcurrentListeners = 10, int awaiterTimeoutInMS = 500)
        {
            this.Port = Port;
            this.MaxConcurrentListeners = MaxConcurrentListeners;
            AwaiterTimeoutInMS = awaiterTimeoutInMS;

            Start();
        }

        public static bool IsIPBanned(string ipAddress, int? clientport)
        {
            if (NetworkLibrary.NetworkLibraryConfiguration.BannedIPs != null && NetworkLibrary.NetworkLibraryConfiguration.BannedIPs.Contains(ipAddress))
            {
                LoggerAccessor.LogError($"[SECURITY] - {ipAddress}:{clientport} Requested the EDEN_UDP server while being banned!");
                return true;
            }

            return false;
        }

        public void Start()
        {
            if (thread != null)
            {
                LoggerAccessor.LogWarn("[EDEN_UDP] - Server already active.");
                return;
            }
            thread = new Thread(Listen);
            thread.Start();
            ClientStore.Start();
            IsStarted = true;
        }

        public void Stop()
        {
            // stop thread and listener
            threadActive = false;
            if (listener != null) listener.Dispose();

            // wait for thread to finish
            if (thread != null)
            {
                thread.Join();
                thread = null;
            }

            // finish closing listener
            if (listener != null)
                listener = null;

            ClientStore.Stop();

            IsStarted = false;
        }

        private void Listen()
        {
            threadActive = true;

            object _sync = new();

            // start listener
            try
            {
                listener = new UdpClient(Port);
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError("[EDEN_UDP] - An Exception Occured while starting the udp client: " + e.Message);
                threadActive = false;
                return;
            }

            LoggerAccessor.LogInfo($"[EDEN_UDP] - Server started on port {Port}...");

            // wait for requests
            while (threadActive)
            {
                lock (_sync)
                {
                    if (!threadActive)
                        break;
                }

                while (UdpTasks.Count < MaxConcurrentListeners) //Maximum number of concurrent listeners
                    UdpTasks.Add(listener.ReceiveAsync().ContinueWith(t =>
                    {
                        UdpReceiveResult? result = null;
                        try
                        {
                            if (!t.IsCompleted)
                                return;
                            result = t.Result;
                        }
                        catch
                        {
                        }
                        _ = ProcessMessagesFromClient(result);
                    }));


                int RemoveAtIndex = Task.WaitAny(UdpTasks.ToArray(), AwaiterTimeoutInMS); //Synchronously Waits up to 500ms for any Task completion
                if (RemoveAtIndex != -1) //Remove the completed task from the list
                    UdpTasks.RemoveAt(RemoveAtIndex);
            }
        }

        #region Protected Functions
        protected virtual Task<bool> ProcessMessagesFromClient(UdpReceiveResult? result)
        {
            if (!result.HasValue)
                return Task.FromResult(false);
#if DEBUG
            LoggerAccessor.LogInfo($"[EDEN_UDP] - Connection received on port {Port} (Thread {Environment.CurrentManagedThreadId.ToString()})");
#endif
            UdpReceiveResult resultVal = result.Value;

            IPEndPoint? endpoint = resultVal.RemoteEndPoint;

            string? clientip = endpoint?.Address.ToString();
            int? clientport = endpoint?.Port;

            if (!clientport.HasValue || string.IsNullOrEmpty(clientip) || IsIPBanned(clientip, clientport))
                return Task.FromResult(false);

            EdStore receivedStore = new EdStore();
            byte[] InData = resultVal.Buffer;

            receivedStore.LoadData(InData, InData.Length);
            ushort initialCrc = receivedStore.ExtractStart();
#if DEBUG
            LoggerAccessor.LogInfo($"[EDEN_UDP] - {clientip} Requested EdStore {initialCrc:X4} : {{{receivedStore.Data.ToHexString().Replace("\n", string.Empty)}}}");
#else
            LoggerAccessor.LogInfo($"[EDEN_UDP] - {clientip} Requested EdStore {initialCrc:X4}");
#endif
            if (!CrcToClass.TryGetValue(initialCrc, out Type? c))
            {
                LoggerAccessor.LogError($"[EDEN_UDP] - {clientip} Requested an unexpected message Type {initialCrc:X4} : SizeOfPacket:{InData.Length}");
                return Task.FromResult(false);
            }

            AbstractMessage? msg = null;

            try
            {
                if (c != null)
                    msg = (AbstractMessage?)Activator.CreateInstance(c);
            }
            catch
            {
            }

            return Task.FromResult(msg?.Process(this, endpoint!, receivedStore) ?? false);
        }
        #endregion
    }
}
