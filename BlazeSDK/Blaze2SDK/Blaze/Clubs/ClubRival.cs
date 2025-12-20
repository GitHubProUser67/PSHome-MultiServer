using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubRival
    {
        
        [TdfMember("CLID")]
        public uint mRivalClubId;
        
        [TdfMember("COP1")]
        public uint mCustOpt1;
        
        [TdfMember("COP2")]
        public uint mCustOpt2;
        
        [TdfMember("COP3")]
        public uint mCustOpt3;
        
        [TdfMember("CRTI")]
        public uint mCreationTime;
        
        [TdfMember("LATI")]
        public uint mLastUpdateTime;
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("META")]
        [StringLength(1024)]
        public string mMetaData;
        
    }
}
