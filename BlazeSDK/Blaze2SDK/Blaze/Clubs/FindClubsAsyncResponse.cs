using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct FindClubsAsyncResponse
    {
        
        [TdfMember("CONT")]
        public uint mCount;
        
        [TdfMember("SQID")]
        public uint mSequenceID;
        
    }
}
