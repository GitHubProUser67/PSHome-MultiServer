using CustomLogger;
using MultiSocks.Aries.Messages;
using MultiSocks.ProtoSSL;
using MultiServerLibrary.Extension;
using System.Net.Sockets;
using System.Text;
using MultiServerLibrary.CustomServers;

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

        private readonly int MaxConcurrentListeners = Environment.ProcessorCount;

        private readonly bool secure = false;
        private readonly bool WeakChainSignedRSAKey = false;
        private readonly string CN = string.Empty;
        private readonly TCPServer? Server;

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

            if (Server == null)
                Server = new TCPServer();
            Server.StartAsync(
                new List<ushort> { port },
                MaxConcurrentListeners,
                null,
                null,
                null,
                (serverPort, client, remoteEP) =>
                {
                    if (remoteEP.AddressFamily == AddressFamily.InterNetworkV6)
                        AddClient(new AriesClient(this, client, this.secure, this.CN, this.WeakChainSignedRSAKey)
                        {
                            ADDR = remoteEP.Address.MapToIPv4().ToString(),
                            SessionID = SessionID++
                        });
                    else
                        AddClient(new AriesClient(this, client, this.secure, this.CN, this.WeakChainSignedRSAKey)
                        {
                            ADDR = remoteEP.Address.ToString(),
                            SessionID = SessionID++
                        });
                },
                new CancellationTokenSource().Token
                );
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
                LoggerAccessor.LogInfo($"[AbstractDirtySockServer] - {client.ADDR} Requested Type {name} : {{{data.ToHexString().Replace("\n", string.Empty)}}}");
#else
                LoggerAccessor.LogInfo($"[AbstractDirtySockServer] - {client.ADDR} Requested Type {name}");
#endif
                if (!NameToClass.TryGetValue(name, out Type? c))
                {
                    LoggerAccessor.LogError($"[AbstractDirtySockServer] - {client.ADDR} Requested an unexpected message Type {name} : {body.Replace("\n", string.Empty)}");
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
            Server?.Stop();
        }
    }
}
