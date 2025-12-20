using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct UpdateUEDRequest
    {
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("CNAM")]
        [StringLength(64)]
        public string mComponentName;
        
        [TdfMember("UED")]
        public int mUserExtendedData;
        
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
