using System.Threading.Tasks;
using CustomLogger;

namespace NetCoreServer.CustomServers
{
    public class NCHTTPServer
    {
        private readonly object _Lock = new object();

        public bool IsStarted { get; private set; } = false;

        private HttpServer _server;
        private HttpsServer _secureServer;

        public void Start(HttpServer server, HttpsServer secureServer)
        {
            lock (_Lock)
            {
                if (IsStarted)
                {
                    LoggerAccessor.LogWarn("[NCHTTP Server] - Server already active.");
                    return;
                }

                _server = server;
                _secureServer = secureServer;

                Parallel.Invoke(
                        () => _server.Start(),
                        () => _secureServer.Start()
                    );

                IsStarted = true;
            }
        }

        public void Stop()
        {
            lock (_Lock)
            {
                if (!IsStarted)
                    return;

                _server?.Dispose();
                _secureServer?.Dispose();

                IsStarted = false;
            }

            LoggerAccessor.LogInfo("[NCHTTP Server] - All listeners stopped.");
        }
    }
}
