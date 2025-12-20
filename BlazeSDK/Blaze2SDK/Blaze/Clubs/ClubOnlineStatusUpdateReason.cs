namespace Blaze2SDK.Blaze.Clubs
{
    public enum ClubOnlineStatusUpdateReason : int
    {
        CLUBS_USER_SESSION_CREATED = 0x0,
        CLUBS_USER_SESSION_DESTROYED = 0x1,
        CLUBS_USER_JOINED_CLUB = 0x2,
        CLUBS_USER_LEFT_CLUB = 0x3,
        CLUBS_USER_ONLINE_STATUS_UPDATED = 0x4,
        CLUBS_CLUB_DESTROYED = 0x5,
        CLUBS_CLUB_CREATED = 0x6,
        CLUBS_CLUB_SETTINGS_UPDATED = 0x7,
        CLUBS_USER_PROMOTED_TO_GM = 0x8,
    }
}
