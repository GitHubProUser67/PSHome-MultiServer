namespace Blaze2SDK.Blaze.Clubs
{
    public enum MemberOnlineStatus : int
    {
        CLUBS_MEMBER_OFFLINE = 0x0,
        CLUBS_MEMBER_ONLINE_INTERACTIVE = 0x1,
        CLUBS_MEMBER_ONLINE_NON_INTERACTIVE = 0x2,
        CLUBS_MEMBER_IN_GAMEROOM = 0x3,
        CLUBS_MEMBER_IN_OPEN_SESSION = 0x4,
        CLUBS_MEMBER_IN_GAME = 0x5,
        CLUBS_MEMBER_CUSTOM_ONLINE_STATUS = 0x6,
    }
}
