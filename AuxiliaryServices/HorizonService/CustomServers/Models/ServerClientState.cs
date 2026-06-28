namespace Horizon.CustomServers.Models
{
    public enum ServerClientState
    {
        DISCONNECTED,
        CONNECTED,
        HELLO,
        HANDSHAKE,
        CONNECT_1,
        AUTHENTICATED,
    }
}
