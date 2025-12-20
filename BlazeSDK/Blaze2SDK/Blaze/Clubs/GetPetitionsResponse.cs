using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct GetPetitionsResponse
    {
        
        [TdfMember("CIST")]
        public List<ClubMessage> mClubPetitionsList;
        
    }
}
