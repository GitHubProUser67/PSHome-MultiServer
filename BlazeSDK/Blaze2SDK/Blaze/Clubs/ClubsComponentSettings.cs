using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubsComponentSettings
    {
        
        [TdfMember("AWST")]
        public List<AwardSettings> mAwardSettings;
        
        [TdfMember("CLDS")]
        public ushort mClubDivisionSize;
        
        [TdfMember("INAC")]
        public ushort mMaxInactiveDays;
        
        [TdfMember("MXEV")]
        public ushort mMaxEvents;
        
        [TdfMember("MXGM")]
        public ushort mMaxGMs;
        
        [TdfMember("MXIN")]
        public ushort mMaxInvitesPerUserOrClub;
        
        [TdfMember("MXME")]
        public ushort mMaxMembers;
        
        [TdfMember("MXNE")]
        public ushort mMaxNews;
        
        [TdfMember("MXRV")]
        public ushort mMaxRivalsPerClub;
        
        [TdfMember("PUHR")]
        public ushort mPurgeHour;
        
        [TdfMember("REST")]
        public List<RecordSettings> mRecordSettings;
        
        [TdfMember("SOVR")]
        public int mSeasonRolloverTime;
        
        [TdfMember("STRT")]
        public int mSeasonStartTime;
        
    }
}
