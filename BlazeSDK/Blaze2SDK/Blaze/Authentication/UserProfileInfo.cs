using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct UserProfileInfo
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CITY")]
        [StringLength(32)]
        public string mCity;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("CTRY")]
        [StringLength(32)]
        public string mCountry;
        
        [TdfMember("GNDR")]
        public Gender mGender;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("STAT")]
        [StringLength(32)]
        public string mState;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("STRT")]
        [StringLength(64)]
        public string mStreet;
        
        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("ZIP")]
        [StringLength(16)]
        public string mZipCode;
        
    }
}
