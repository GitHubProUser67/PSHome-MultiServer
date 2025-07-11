using System.Net.Sockets;

namespace MultiServerLibrary.Extension
{
    public static class TcpClientUtils
    {
        public static bool IsConnected(this TcpClient tcpClient)
        {
            if (tcpClient.Client.Connected && tcpClient.Client.Poll(0, SelectMode.SelectWrite) && !tcpClient.Client.Poll(0, SelectMode.SelectError))
                return !(tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

            return false;
        }
    }
}
