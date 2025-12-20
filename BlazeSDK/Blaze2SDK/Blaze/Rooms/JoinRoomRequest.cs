using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct JoinRoomRequest
    {
        
        [TdfMember("CTID")]
        public uint mCategoryId;
        
        [TdfMember("INID")]
        public uint mInviterId;
        
        [TdfMember("INVT")]
        public bool mIsUserInvited;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(16)]
        public string mPassword;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("PVAL")]
        [StringLength(32)]
        public string mPseudoValue;
        
        [TdfMember("RMID")]
        public uint mRoomId;
        
    }
}
