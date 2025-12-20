using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameSizeRulePrefs
    {
        
        [TdfMember("ISSG")]
        public byte mIsSingleGroupMatch;
        
        [TdfMember("PCAP")]
        public ushort mMaxPlayerCapacity;
        
        [TdfMember("PCNT")]
        public ushort mDesiredPlayerCount;
        
        [TdfMember("PMIN")]
        public ushort mMinPlayerCount;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("THLD")]
        [StringLength(32)]
        public string mMinFitThresholdName;
        
    }
}
