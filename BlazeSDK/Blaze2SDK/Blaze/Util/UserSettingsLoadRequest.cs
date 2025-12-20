using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct UserSettingsLoadRequest
    {
        
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
