using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameAttributeCensusData
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ATTN")]
        [StringLength(32)]
        public string mAttributeName;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("ATTV")]
        [StringLength(256)]
        public string mAttributevalue;
        
        [TdfMember("NOFG")]
        public uint mNumOfGames;
        
        [TdfMember("NOFP")]
        public uint mNumOfPlayers;
        
    }
}
