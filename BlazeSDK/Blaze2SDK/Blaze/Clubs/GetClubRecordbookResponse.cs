using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetClubRecordbookResponse
    {
        
        [TdfMember("CLRL")]
        public List<ClubRecord> mClubRecordList;
        
    }
}
