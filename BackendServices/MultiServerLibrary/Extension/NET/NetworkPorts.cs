namespace System.Net
{
    public static class NetworkPorts
    {
        public static class Dns
        {
            public const ushort Udp = 53;
            public const ushort Tcp = Udp;
        }

        public static class Http
        {
            public const ushort Tcp = 80;
            public const ushort TcpAux = 8080;
            public const ushort Ssl = 443;
            public const ushort SslAux = 8443;
        }
    }
}
