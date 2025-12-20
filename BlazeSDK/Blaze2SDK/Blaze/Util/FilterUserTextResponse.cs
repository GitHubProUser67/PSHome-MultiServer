using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct FilterUserTextResponse
    {
        
        [TdfMember("TLST")]
        public List<FilteredUserText> mFilteredTextList;
        
    }
}
