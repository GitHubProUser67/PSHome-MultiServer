using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct AssociationListInfo
    {
        
        [TdfMember("ALML")]
        public List<ListMemberInfo> mListMemberInfoVector;
        
        [TdfMember("BOID")]
        public ulong mBlazeObjId;
        
        [TdfMember("LID")]
        public uint mId;
        
        [TdfMember("LMS")]
        public uint mMaxSize;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("LNM")]
        [StringLength(32)]
        public string mName;
        
        [TdfMember("RFLG")]
        public bool mRollover;
        
        [TdfMember("SFLG")]
        public bool mSubscprition;
        
    }
}
