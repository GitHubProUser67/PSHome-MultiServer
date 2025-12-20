using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct CheckAgeReqRequest
    {
        
        [TdfMember("BDAY")]
        public int mBirthDay;
        
        [TdfMember("BMON")]
        public int mBirthMonth;
        
        [TdfMember("BYR")]
        public int mBirthYear;
        
        /// <summary>
        /// Max String Length: 4
        /// </summary>
        [TdfMember("CTRY")]
        [StringLength(4)]
        public string mIsoCountryCode;
        
    }
}
