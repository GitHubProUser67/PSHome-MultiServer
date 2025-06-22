using System.Net;

namespace NthDeveloper.TelnetServer
{
    public delegate void ClientConnectionEventHandler(ITCPServer server, ClientInfo client);
    public delegate void ClientDataEventHandler(ITCPServer server, ClientInfo client, byte[] data);

    /// <summary>
    /// Multi client TCP Server interface
    /// </summary>
    public interface ITCPServer
    {
        /// <summary>
        /// Number of max connection requests on the queue
        /// </summary>
        int MaxWaitingClients { get; set; }

        /// <summary>
        /// Default buffer size for reading from clients
        /// </summary>
        int ReadBufferSize { get; set; }

        /// <summary>
        /// Occures when a client is connected
        /// </summary>
        event ClientConnectionEventHandler ClientConnected;

        /// <summary>
        /// Occures when a client is disconnected
        /// </summary>
        event ClientConnectionEventHandler ClientDisconnected;

        /// <summary>
        /// Occures when data received from a client
        /// </summary>
        event ClientDataEventHandler DataReceived;

        /// <summary>
        /// Starts to listen for incoming connections
        /// </summary>
        /// <param name="endPoint"></param>
        void StartListening(IPEndPoint endPoint);

        /// <summary>
        /// Stops listening for incoming connections
        /// </summary>
        void StopListening();
    }
}