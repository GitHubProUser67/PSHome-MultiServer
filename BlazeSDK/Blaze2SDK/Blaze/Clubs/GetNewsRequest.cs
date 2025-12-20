using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetNewsRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("MCNT")]
        public uint mMaxResultCount;
        
        [TdfMember("NSOT")]
        public TimeSortType mSortType;
        
        [TdfMember("OFST")]
        public uint mOffSet;
        
        [TdfMember("TFIL")]
        public List<NewsType> mTypeFilters;
        
    }
}
