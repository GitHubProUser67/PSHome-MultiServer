using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Arson
{
    [TdfStruct]
    public struct AddPointsToWalletRequest
    {
        
        [TdfMember("AMT")]
        public uint mAmount;
        
        /// <summary>
        /// Max String Length: 255
        /// </summary>
        [TdfMember("WNM")]
        [StringLength(255)]
        public string mWalletName;
        
    }
}
