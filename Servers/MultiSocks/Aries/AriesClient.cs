using CustomLogger;
using FixedSsl;
using MultiSocks.Aries.Messages;
using MultiSocks.Aries.Model;
using Org.BouncyCastle.Crypto;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MultiSocks.Aries
{
    public class AriesClient
    {
        public AbstractAriesServer Context;
        public AriesUser? User;
        public CancellationTokenSource? QuickJoinTaskTokenSource;
        public Task? QuickJoinTask;
        public string ADDR = "127.0.0.1";
        public string LADDR = "127.0.0.1";
        public string VERS = string.Empty;
        public string SKU = string.Empty;
        public string SKEY = string.Empty;
        public string? Port = null;
        public int SessionID;
        public bool CanAsyncGameSearch = false;
        public bool CanAsync = true;
        public bool Disconnected = false;

        private readonly bool secure;
        private readonly SemaphoreSlim dequeueSemaphore = new(1, 1);
        private readonly TcpClient tcpClient;
        private readonly Thread RecvThread;
        private readonly ConcurrentQueue<AbstractMessage> AsyncMessageQueue = new();
        private Stream? ClientStream;
        private string CommandName = "null";
        private uint ErrorCode = 0;

        private (AsymmetricKeyParameter, Org.BouncyCastle.Tls.Certificate, X509Certificate2) SecureKeyCert;

        public long PingSendTick;
        public int Ping;

        private const int MAX_SIZE = 1024 * 1024 * 2;

        public AriesClient(AbstractAriesServer context, TcpClient client, bool secure, string CN, bool WeakChainSignedRSAKey)
        {
            this.secure = secure;
            Context = context;
            tcpClient = client;

            LoggerAccessor.LogInfo("[AriesClient] - New connection from " + ADDR + ".");

            if (secure && context.SSLCache != null)
            {
                if (CN == "fesl.ea.com")
                    SecureKeyCert = context.SSLCache.GetVulnerableFeslEaCert();
                else
                    SecureKeyCert = context.SSLCache.GetVulnerableLegacyCustomEaCert(CN, WeakChainSignedRSAKey);
            }

            RecvThread = new Thread(RunLoop);
            RecvThread.Start();
        }

        private async void RunLoop()
        {
            try
            {
#pragma warning disable
                ClientStream = await SslSocket.AuthenticateAsServerAsync(SslProtocols.Ssl3, tcpClient.Client, SecureKeyCert.Item3, secure, true).ConfigureAwait(false);
#pragma warning restore
            }
            catch (Exception e)
            {
                ClientStream?.Dispose();
                tcpClient.Dispose();
                Disconnected = true;
                LoggerAccessor.LogError($"[AriesClient] - Failed to accept connection, User {ADDR} forced disconnected. (Exception:{e})");
                Context.RemoveClient(this);

                return;
            }

            bool InHeader = false;
            int len, TempDatOff = 0;
            int ExpectedBytes = -1;
            byte[]? TempData = null;
            byte[] bytes = new byte[65536];

            try
            {
                // Do not use the async equivalent of ClientStream.Read or issues will happen.
                while ((len = ClientStream.Read(bytes)) != 0)
                {
                    int off = 0;
                    while (len > 0)
                    {
                        // got some data
                        if (ExpectedBytes == -1)
                        {
                            // new packet
                            InHeader = true;
                            ExpectedBytes = 12; // header
                            TempData = new byte[12];
                            TempDatOff = 0;
                        }

                        if (TempData != null)
                        {
                            int copyLen = Math.Min(len, TempData.Length - TempDatOff);
                            Array.Copy(bytes, off, TempData, TempDatOff, copyLen);
                            off += copyLen;
                            TempDatOff += copyLen;
                            len -= copyLen;

                            if (TempDatOff == TempData.Length)
                            {
                                if (InHeader)
                                {
                                    // header complete.
                                    InHeader = false;
                                    int size = TempData[11] | TempData[10] << 8 | TempData[9] << 16 | TempData[8] << 24;
                                    if (size > MAX_SIZE)
                                    {
                                        tcpClient.Close(); // either something terrible happened or they're trying to mess with us
                                        break;
                                    }
                                    CommandName = Encoding.ASCII.GetString(TempData)[..4];
                                    ErrorCode = (uint)(TempData[7] | TempData[6] << 8 | TempData[5] << 16 | TempData[4] << 24);

                                    TempData = new byte[size - 12];
                                    TempDatOff = 0;
                                }
                                else
                                {
                                    // message complete, process in a sync manner to avoids issues.
                                    GotMessage(CommandName, ErrorCode, TempData.ToArray());

                                    TempDatOff = 0;
                                    ExpectedBytes = -1;
                                    TempData = null;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Not Important.
            }

            ClientStream?.Dispose();
            tcpClient.Dispose();
            Disconnected = true;
            LoggerAccessor.LogWarn($"[AriesClient] - User {ADDR} disconnected.");
            Context.RemoveClient(this);
        }

        private void GotMessage(string name, uint errorCode, byte[] data)
        {
            Task.Run(() =>
            {
                Context.HandleMessage(name, errorCode, data, this);
            }).Wait();
        }

        private Task DequeueAsyncMessage()
        {
            if (!dequeueSemaphore.Wait(0))
                return Task.CompletedTask;

            try
            {
                while (AsyncMessageQueue.TryDequeue(out AbstractMessage? msg))
                {
                    // Some games not like when async msgs are sent too close to each others (MOH).
                    Thread.Sleep(100);

                    if (msg != null)
                        SendImmediateMessage(msg.GetData());
                }
            }
            finally
            {
                dequeueSemaphore.Release();
            }

            return Task.CompletedTask;
        }

        public void StopGameQuickSearch()
        {
            QuickJoinTaskTokenSource?.Cancel();
        }

        public bool SendImmediateMessage(byte[] data)
        {
            if (ClientStream != null)
            {
                try
                {
                    ClientStream.Write(data, 0, data.Length);

                    return true;
                }
                catch
                {
                    // something bad happened :(
                }
            }

            return false;
        }

        public void SendMessage(AbstractMessage msg)
        {
            if ("+gam".Equals(msg._Name) && !CanAsyncGameSearch)
                return;
            else if (msg._Name.StartsWith('+'))
            {
                if (CanAsync)
                    AsyncMessageQueue.Enqueue(msg);

                _ = DequeueAsyncMessage();

                return;
            }

            try
            {
                byte[] data = msg.GetData();
                ClientStream?.Write(data, 0, data.Length);
            }
            catch
            {
                // something bad happened :(
            }
        }

        public bool HasAuth()
        {
            return User != null;
        }
    }
}
