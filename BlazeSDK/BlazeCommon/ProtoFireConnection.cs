using Org.Mentalis.Security.Ssl;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace BlazeCommon
{
    public class ProtoFireConnection
    {
        public long ID { get; }
        public ProtoFireServer? Owner { get; }
        public MitmProtoFireServer? OwnerMitm { get; }
        public Socket Socket { get; }
        public Stream? Stream { get; private set; }
        public bool Connected { get; private set; }

        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public ProtoFireConnection(long id, ProtoFireServer owner, Socket socket)
        {
            ID = id;
            Owner = owner;
            Socket = socket;
            Stream = null;
            Connected = true;
        }

        public ProtoFireConnection(long id, MitmProtoFireServer owner, Socket socket)
        {
            ID = id;
            OwnerMitm = owner;
            Socket = socket;
            Stream = null;
            Connected = true;
        }

        public ProtoFireConnection(Socket socket)
        {
            ID = 0;
            Owner = null;
            Socket = socket;
            Stream = null;
            Connected = true;
        }

        public void SetStream(Stream stream)
        {
            if (Stream != null)
                throw new InvalidOperationException("Stream is already set");
            Stream = stream;
        }

        public void Disconnect()
        {
            if (!Connected)
                return;

            Connected = false;

            //stream owns the socket, so no need to close the socket
            try { Stream?.Close(); } catch { }

            Owner?.KillConnection(this); //remove from connection list
            OwnerMitm?.KillConnection(this); //remove from connection list
        }


        public async Task<ProtoFirePacket?> ReadPacketAsync()
        {
            if (!Connected)
                return null;

            if (Stream == null)
                throw new InvalidOperationException("Stream is not set");

            try
            {
                FireFrame frame = new FireFrame();
                if (!await Stream.ReadAllAsync(frame.Frame, 0, FireFrame.MIN_HEADER_SIZE).ConfigureAwait(false))
                    return null;

                ushort extraFrameBytesNeeded = frame.ExtraHeaderSize;
                if (!await Stream.ReadAllAsync(frame.Frame, FireFrame.MIN_HEADER_SIZE, extraFrameBytesNeeded).ConfigureAwait(false))
                    return null;

                byte[] data = new byte[frame.Size];
                if (!await Stream.ReadAllAsync(data, 0, data.Length).ConfigureAwait(false))
                    return null;

                return new ProtoFirePacket(frame, data);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ProtoFirePacket? ReadPacket()
        {
            if (!Connected)
                return null;

            if (Stream == null)
                throw new InvalidOperationException("Stream is not set");

            try
            {

                FireFrame frame = new FireFrame();
                if (!Stream.ReadAll(frame.Frame, 0, FireFrame.MIN_HEADER_SIZE))
                    return null;

                ushort extraFrameBytesNeeded = frame.ExtraHeaderSize;
                if (!Stream.ReadAll(frame.Frame, FireFrame.MIN_HEADER_SIZE, extraFrameBytesNeeded))
                    return null;

                byte[] data = new byte[frame.Size];
                if (!Stream.ReadAll(data, 0, data.Length))
                    return null;

                return new ProtoFirePacket(frame, data);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public bool Send(ProtoFirePacket packet)
        {
            if (!Connected)
                return false;

            if (Stream == null)
                throw new InvalidOperationException("Stream is not set");

            bool success = false;

            semaphoreSlim.Wait();
            try
            {
                packet.WriteTo(Stream);
                Stream.Flush();
                success = true;
            }
            catch (ObjectDisposedException)
            {
                success = false;
            }
            catch (IOException)
            {
                success = false;
            }
            finally
            {
                semaphoreSlim.Release();
            }
            return success;
        }

        public async Task<bool> SendAsync(ProtoFirePacket packet)
        {
            if (!Connected)
                return false;

            if (Stream == null)
                throw new InvalidOperationException("Stream is not set");

            bool success = false;
            await semaphoreSlim.WaitAsync();
            try
            {
                await packet.WriteToAsync(Stream).ConfigureAwait(false);
                await Stream.FlushAsync().ConfigureAwait(false);
                success = true;
            }
            catch (ObjectDisposedException)
            {
                success = false;
            }
            catch (IOException)
            {
                success = false;
            }
            finally
            {
                semaphoreSlim.Release();
            }
            return success;
        }



        private static async Task<Socket?> ConnectToAsync(string hostname, int port)
        {
            IPHostEntry host = Dns.GetHostEntry(hostname);
            if (host.AddressList.Length == 0)
                return null;

            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP  socket.
            Socket sock = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                await sock.ConnectAsync(remoteEP).ConfigureAwait(false);
                return sock;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<ProtoFireConnection?> ConnectAsync(string hostname, int port, bool ssl = true)
        {
            Socket? sock = await ConnectToAsync(hostname, port).ConfigureAwait(false);
            if (sock == null)
                return null;

            Stream stream = new NetworkStream(sock, true);
            if (ssl)
            {
                SslStream sslStream = new SslStream(stream, false, RemoteCertificateVerify);
                await sslStream.AuthenticateAsClientAsync(hostname, null, System.Security.Authentication.SslProtocols.Tls, false).ConfigureAwait(false);
                stream = sslStream;
            }

            var ret = new ProtoFireConnection(sock);
            ret.SetStream(stream);
            return ret;
        }

        public static ProtoFireConnection? ConnectSsl3(string hostname, int port)
        {
            IPHostEntry host = Dns.GetHostEntry(hostname);
            if (host.AddressList.Length == 0)
                return null;

            SecurityOptions options = new SecurityOptions(
                SecureProtocol.Ssl3 | SecureProtocol.Tls1,  // use SSL3 or TLS1
                null!,                                       // do not use client authentication
                ConnectionEnd.Client,                       // this is the client side
                CredentialVerification.None,                // do not check the certificate -- this should not be used in a real-life application :-)
                null!,                                       // not used with automatic certificate verification
                hostname,                        // this is the common name of the Microsoft web server
                SecurityFlags.Default,                      // use the default security flags
                SslAlgorithms.ALL,               // only use secure ciphers
                null!);										// do not process certificate requests.

            SecureSocket s = new SecureSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, options);
            // connect to the remote host
            s.Connect(new IPEndPoint(host.AddressList[0], port));


            ProtoFireConnection connection = new ProtoFireConnection(null!);
            connection.SetStream(new SecureNetworkStream(s, true));
            return connection;
        }

        public static ProtoFireConnection? ConnectSsl3(long address, int port)
        {
            SecurityOptions options = new SecurityOptions(
                SecureProtocol.Ssl3 | SecureProtocol.Tls1,  // use SSL3 or TLS1
                null!,                                       // do not use client authentication
                ConnectionEnd.Client,                       // this is the client side
                CredentialVerification.None,                // do not check the certificate -- this should not be used in a real-life application :-)
                null!,                                       // not used with automatic certificate verification
                null!,                        // this is the common name of the Microsoft web server
                SecurityFlags.Default,                      // use the default security flags
                SslAlgorithms.SECURE_CIPHERS,               // only use secure ciphers
                null!);										// do not process certificate requests.

            SecureSocket s = new SecureSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, options);
            // connect to the remote host
            s.Connect(new IPEndPoint(address, port));


            ProtoFireConnection connection = new ProtoFireConnection(null!);
            connection.SetStream(new SecureNetworkStream(s, true));
            return connection;
        }

        private static bool RemoteCertificateVerify(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
