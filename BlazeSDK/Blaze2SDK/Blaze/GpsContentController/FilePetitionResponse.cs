using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GpsContentController
{
    [TdfStruct]
    public struct FilePetitionResponse
    {
        
        /// <summary>
        /// Max String Length: 36
        /// </summary>
        [TdfMember("GUID")]
        [StringLength(36)]
        public string mPetitionGuid;
        
    }
}
