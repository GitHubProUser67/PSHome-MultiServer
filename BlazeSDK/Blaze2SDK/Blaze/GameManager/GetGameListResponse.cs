using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GetGameListResponse
    {
        
        [TdfMember("GLID")]
        public uint mListId;
        
        [TdfMember("MAXF")]
        public uint mMaxPossibleFitScore;
        
    }
}
