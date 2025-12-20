namespace Blaze3SDK.Blaze.GameManager
{
    [Flags]
    public enum PlayerNetConnectionFlags
    {
        None = 0,
        _DEPRECATED = 1, //DEPRECATED, previously hosted game server flag
        ConnectionDemangled = 2,
        ConnectionPktReceived = 4,
    }
}