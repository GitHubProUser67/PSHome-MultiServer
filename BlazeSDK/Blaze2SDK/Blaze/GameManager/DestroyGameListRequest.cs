using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct DestroyGameListRequest
    {
        
        [TdfMember("GLID")]
        public uint mListId;
        
    }
}
