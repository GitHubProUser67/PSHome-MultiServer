using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace NthDeveloper.TelnetServer
{
    public sealed class TCPServer : ITCPServer
    {
        private Socket m_ServerSocket;
        private List<ClientInfo> m_Clients;        

        public int MaxWaitingClients { get; set; }
        public int ReadBufferSize { get; set; }

        public event ClientConnectionEventHandler ClientConnected;
        public event ClientConnectionEventHandler ClientDisconnected;
        public event ClientDataEventHandler DataReceived;

        public TCPServer()
        {
            MaxWaitingClients = 100;
            ReadBufferSize = 1024;
        }

        public void StartListening(IPEndPoint endPoint)
        {
            m_Clients = new List<ClientInfo>(4);
            // Create a TCP/IP socket.
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                m_ServerSocket.Bind(endPoint);
                m_ServerSocket.Listen(MaxWaitingClients);
                m_ServerSocket.BeginAccept(new AsyncCallback(acceptCallback), m_ServerSocket);
            }
            catch
            {
                StopListening();

                throw;
            }
        }

        public void StopListening()
        {
            if (m_ServerSocket == null)
                return;

            //Close server socket
            try
            {
                m_ServerSocket.Close();                
            }
            catch{ }

            m_ServerSocket = null;

            //Close client sockets
            while (m_Clients.Count > 0)
            {
                var _client = m_Clients[0];

                try
                {
                    _client.ClientSocket.Close();                    
                }
                catch { }

                m_Clients.RemoveAt(0);
            }
        }

        private void acceptCallback(IAsyncResult ar)
        {
            try
            {
                if (m_ServerSocket == null)
                    return;

                // 
                Socket _clientSocket = m_ServerSocket.EndAccept(ar);

                if (_clientSocket == null)
                    return;               

                // Create the state object.
                ClientInfo _clientInfo = new ClientInfo(System.Guid.NewGuid().ToString(), _clientSocket, new byte[this.ReadBufferSize]);

                var _ev = ClientConnected;
                if (_ev != null)
                    _ev(this, _clientInfo);

                _clientSocket.BeginReceive(_clientInfo.Buffer, 0, this.ReadBufferSize, 0, new AsyncCallback(readCallback), _clientInfo);                

                m_ServerSocket.BeginAccept(new AsyncCallback(acceptCallback), m_ServerSocket);
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
               //TODO: Log
            }
        }

        private void readCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                ClientInfo _clientInfo = (ClientInfo)ar.AsyncState;
                Socket _clientSocket = _clientInfo.ClientSocket;

                // Read data from the client socket. 
                int _bytesRead = _clientSocket.EndReceive(ar);

                if (_bytesRead > 0)
                {
                    byte[] _data = new byte[_bytesRead];
                    Array.Copy(_clientInfo.Buffer, 0, _data, 0, _bytesRead);

                    var _ev = DataReceived;
                    if (_ev != null)
                        _ev(this, _clientInfo, _data);

                    try
                    { _clientSocket.BeginReceive(_clientInfo.Buffer, 0, this.ReadBufferSize, 0, new AsyncCallback(readCallback), _clientInfo); }
                    catch { }
                }
                else
                {
                    //Disconnected
                    var _ev = ClientDisconnected;
                    if (_ev != null)
                        _ev(this, _clientInfo);
                }
            }
            catch(ObjectDisposedException) { }
            catch(Exception e)
            {
                //TODO: Log
            }
        }
    }
}
