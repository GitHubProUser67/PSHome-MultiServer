using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct UserSettingsSaveRequest
    {
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("DATA")]
        [StringLength(1024)]
        public string mData;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("KEY")]
        [StringLength(32)]
        public string mKey;
        
        [TdfMember("UID")]
        public uint mUserId;
        
    }
}
