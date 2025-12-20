using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct GetUEDRequest
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("CNAM")]
        [StringLength(64)]
        public string mComponentName;
        
        [TdfMember("UEDK")]
        public ushort mUserExtendedDataId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("UEDN")]
        [StringLength(64)]
        public string mUserExtendedDataName;
        
    }
}
