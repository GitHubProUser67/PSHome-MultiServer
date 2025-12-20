using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct SubmitStatisticsRequest
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CATE")]
        [StringLength(32)]
        public string mCategory;
        
        [TdfMember("CTXT")]
        public int mContext;
        
        [TdfMember("ENTS")]
        public List<uint> mEntities;
        
        [TdfMember("NENT")]
        public int mNumEntities;
        
        [TdfMember("NSTA")]
        public int mNumStats;
        
        [TdfMember("STAT")]
        public List<int> mStats;
        
    }
}
