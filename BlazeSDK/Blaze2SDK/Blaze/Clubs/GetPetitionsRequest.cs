using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetPetitionsRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("INVT")]
        public PetitionsType mPetitionsType;
        
        [TdfMember("NSOT")]
        public TimeSortType mSortType;
        
    }
}
