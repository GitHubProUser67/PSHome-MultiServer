using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct KeyScopedStatValues
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("GRNM")]
        [StringLength(32)]
        public string mGroupName;
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("KEY")]
        [StringLength(1024)]
        public string mKeyString;
        
        [TdfMember("LAST")]
        public bool mLast;
        
        [TdfMember("STS")]
        public StatValues mStatValues;
        
        [TdfMember("VID")]
        public uint mViewId;
        
    }
}
