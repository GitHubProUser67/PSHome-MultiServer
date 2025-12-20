using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ResetClubRecordsRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("RCID")]
        public List<uint> mRecordIdList;
        
    }
}
