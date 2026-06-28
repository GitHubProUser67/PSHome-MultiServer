using System.Net.Sockets;

namespace MultiServerLibrary.Extension
{
    public static class TcpClientUtils
    {
        [ThreadStatic]
        private static byte[]? _peekBuffer;

        extension(TcpClient tcpClient)
        {
            public bool IsConnected()
            {
                _peekBuffer ??= new byte[1]; // Only allocate in this function to not cause unnecessary memory load when using other funcs in the same class.

                return tcpClient.Client.Connected
                    && tcpClient.Client.Poll(0, SelectMode.SelectWrite)
                    && !tcpClient.Client.Poll(0, SelectMode.SelectError)
                    && !(tcpClient.Client.Receive(_peekBuffer, SocketFlags.Peek) == 0);
            }
        }

        public static async Task<bool> TryConnectAsync(
            string host,
            ushort port,
            int timeoutMs = 5000
        )
        {
            using var client = new TcpClient();
            using var cts = new CancellationTokenSource(timeoutMs);

            try
            {
                await client.ConnectAsync(host, port, cts.Token).ConfigureAwait(false);
                return true;
            }
            catch
            {
                // timeout or error
            }

            return false;
        }
    }
}
