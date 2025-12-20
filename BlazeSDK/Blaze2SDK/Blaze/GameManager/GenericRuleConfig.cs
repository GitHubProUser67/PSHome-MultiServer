using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GenericRuleConfig
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("ANME")]
        [StringLength(32)]
        public string mAttributeName;
        
        [TdfMember("ATYP")]
        public GenericRuleAttributeType mAttributeType;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("POSV")]
        public List<string> mPossibleValues;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("RNME")]
        [StringLength(32)]
        public string mRuleName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("THLS")]
        public List<string> mThresholdNames;
        
        [TdfMember("WGHT")]
        public uint mWeight;
        
        public enum GenericRuleAttributeType : int
        {
            PLAYER_ATTRIBUTE = 0x0,
            GAME_ATTRIBUTE = 0x1,
        }
        
    }
}
