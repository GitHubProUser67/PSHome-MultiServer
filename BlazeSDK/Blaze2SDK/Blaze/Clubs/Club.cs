using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct Club
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("CLIN")]
        public ClubInfo mClubInfo;
        
        [TdfMember("CLST")]
        public ClubSettings mClubSettings;
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(30)]
        public string mName;
        
        public enum ClubConfigParams : int
        {
            CLUBS_MAX_MEMBER_COUNT = 0x0,
            CLUBS_MAX_GM_COUNT = 0x1,
            CLUBS_MAX_DIVISION_SIZE = 0x2,
            CLUBS_MAX_NEWS_COUNT = 0x3,
            CLUBS_MAX_INVITE_COUNT = 0x4,
            CLUBS_MAX_INACTIVE_DAYS = 0x5,
            CLUBS_PURGE_HOUR = 0x6,
        }
        
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
}
