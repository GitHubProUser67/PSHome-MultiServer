using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct UserStringList
    {
        
        [TdfMember("UTXT")]
        public List<UserText> mTextList;
        
    }
}
