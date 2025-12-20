using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct UpdateClubSettingsRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("CLST")]
        public ClubSettings mClubSettings;
        
    }
}
