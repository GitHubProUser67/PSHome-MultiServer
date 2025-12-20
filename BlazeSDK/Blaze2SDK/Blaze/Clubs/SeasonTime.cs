using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct SeasonTime
    {
        
        [TdfMember("SOVR")]
        public int mSeasonRolloverTime;
        
        [TdfMember("STRT")]
        public int mSeasonStartTime;
        
    }
}
