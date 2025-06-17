using CustomLogger;
using MultiSocks.Aries.Messages;
using MultiSocks.ProtoSSL;
using NetworkLibrary.Extension;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiSocks.Aries
{
    public abstract class AbstractAriesServer : IDisposable
    {
        public abstract Dictionary<string, Type?> NameToClass { get; }
        public string? Project = null;
        public string? SKU = null;
        public ushort listenPort;
        public string listenIP;
        public int SessionID = 1;
        public VulnerableCertificateGenerator? SSLCache = null;
        public List<AriesClient> DirtySocksClients = new();
        public TcpListener? Listener;

        private List<Task> TcpClientTasks = new();
        private readonly int AwaiterTimeoutInMS = 500;
        private int MaxConcurrentListeners = 10;
        private volatile bool threadActive;
        private bool secure = false;
        private bool WeakChainSignedRSAKey = false;
        private string CN = string.Empty;
        private Thread? ListenerThread;

        public AbstractAriesServer(ushort port, string listenIP, string? Project = null, string? SKU = null, bool secure = false, string CN = "", bool WeakChainSignedRSAKey = false)
        {
            listenPort = port;
            this.listenIP = listenIP;
            this.secure = secure;
            this.WeakChainSignedRSAKey = WeakChainSignedRSAKey;
            this.CN = CN;
            this.Project = Project;
            this.SKU = SKU;

            if (secure)
                SSLCache = new();

            ListenerThread = new Thread(RunLoop);
            ListenerThread.Start();
        }

        private void RunLoop()
        {
            threadActive = true;

            object _sync = new object();

            try
            {
                Listener = new TcpListener(IPAddress.Any, listenPort);
                Listener.Start();
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError("[AbstractAriesServer] - An Exception Occured while starting the DirtySock listener: " + e.Message);
                threadActive = false;
                return;
            }

            LoggerAccessor.LogInfo($"[AbstractAriesServer] - DirtySock Server started on port {listenPort}...");

            // wait for requests
            while (threadActive)
            {
                lock (_sync)
                {
                    if (!threadActive)
                        break;
                }

                while (TcpClientTasks.Count < MaxConcurrentListeners) //Maximum number of concurrent listeners
                    TcpClientTasks.Add(Listener!.AcceptTcpClientAsync().ContinueWith(t =>
                    {
                        TcpClient? client = null;
                        try
                        {
                            if (!t.IsCompleted)
                                return;

                            client = t.Result;
                        }
                        catch
                        {
                        }
                        _ = Task.Run(() => {
                            if (client != null && client.Client.RemoteEndPoint is IPEndPoint remoteEndPoint)
                            {
#if DEBUG
                                LoggerAccessor.LogInfo($"[AbstractAriesServer] - Connection received (Thread " + Thread.CurrentThread.ManagedThreadId.ToString() + ")");
#endif
                                if (remoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                                    AddClient(new AriesClient(this, client, secure, CN, WeakChainSignedRSAKey)
                                    {
                                        ADDR = remoteEndPoint.Address.MapToIPv4().ToString(),
                                        SessionID = SessionID++
                                    });
                                else
                                    AddClient(new AriesClient(this, client, secure, CN, WeakChainSignedRSAKey)
                                    {
                                        ADDR = remoteEndPoint.Address.ToString(),
                                        SessionID = SessionID++
                                    });
                            }
                        });
                    }));

                int RemoveAtIndex = Task.WaitAny(TcpClientTasks.ToArray(), AwaiterTimeoutInMS); //Synchronously Waits up to 500ms for any Task completion
                if (RemoveAtIndex != -1) //Remove the completed task from the list
                    TcpClientTasks.RemoveAt(RemoveAtIndex);
            }
        }

        public virtual void AddClient(AriesClient client)
        {
            lock (DirtySocksClients)
                DirtySocksClients.Add(client);
        }

        public virtual void RemoveClient(AriesClient client)
        {
            lock (DirtySocksClients)
                DirtySocksClients.Remove(client);
        }

        public void Broadcast(AbstractMessage msg)
        {
            lock (DirtySocksClients)
            {
                foreach (AriesClient user in DirtySocksClients)
                {
                    user.PingSendTick = DateTime.Now.Ticks;
                    user.SendMessage(msg);
                }
            }
        }

        public virtual void HandleMessage(string name, uint errorCode, byte[] data, AriesClient client)
        {
            try
            {
                string body = Encoding.ASCII.GetString(data);
#if DEBUG
                LoggerAccessor.LogInfo($"{client.ADDR} Requested Type {name} : {{{data.ToHexString().Replace("\n", string.Empty)}}}");
#else
                LoggerAccessor.LogInfo($"{client.ADDR} Requested Type {name}");
#endif
                if (!NameToClass.TryGetValue(name, out Type? c))
                {
                    LoggerAccessor.LogError($"{client.ADDR} Requested an unexpected message Type {name} : {body.Replace("\n", string.Empty)}");
                    return;
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

                if (msg != null)
                {
                    msg.Read(body);
                    msg.Process(this, client);
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[AbstractDirtySockServer] - HandleMessage thrown an exception : {ex}");
            }
        }

        public void Dispose()
        {
            // stop thread and listener
            threadActive = false;

            if (Listener != null)
                Listener.Stop();

            // wait for thread to finish
            if (ListenerThread != null)
            {
                ListenerThread.Join();
                ListenerThread = null;
            }

            // finish closing listener
            if (Listener != null)
                Listener = null;
        }
    }
}
