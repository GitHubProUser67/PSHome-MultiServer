using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct UpdateListResponse
    {
        
        [TdfMember("BIDL")]
        public List<ListMemberInfo> mListMemberInfoVector;
        
        [TdfMember("RBDL")]
        public List<ListMemberInfo> mListRemovedMemberInfoVector;
        
    }
}
