using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetDivisionResponse
    {
        
        [TdfMember("DIVN")]
        public uint mDivision;
        
        [TdfMember("SRNK")]
        public uint mStartingRank;
        
    }
}
