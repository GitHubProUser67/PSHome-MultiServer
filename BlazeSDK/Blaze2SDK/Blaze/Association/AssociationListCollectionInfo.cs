using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct AssociationListCollectionInfo
    {
        
        /// <summary>
        /// Max Key String Length: 32
        /// </summary>
        [TdfMember("ALMP")]
        public SortedDictionary<string, AssociationListInfo> mAssociationListInfoByNameMap;
        
    }
}
