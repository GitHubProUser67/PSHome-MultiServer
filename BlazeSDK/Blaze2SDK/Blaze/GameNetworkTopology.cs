namespace Blaze2SDK.Blaze
{
    public enum GameNetworkTopology : int
    {
        CLIENT_SERVER_PEER_HOSTED = 0x0,
        CLIENT_SERVER_DEDICATED = 0x1,
        PEER_TO_PEER_FULL_MESH = 0x82,
        PEER_TO_PEER_PARTIAL_MESH = 0x83,
        PEER_TO_PEER_DIRTYCAST_FAILOVER = 0x84,
    }
}
