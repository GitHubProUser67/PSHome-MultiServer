using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct GetLeaguesByUserRequest
    {
        
        [TdfMember("ONLN")]
        public byte mFindNumberOfMembersOnline;
        
    }
}
