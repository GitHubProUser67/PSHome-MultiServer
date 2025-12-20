using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct Entitlement
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("GDAY")]
        [StringLength(32)]
        public string mGrantDate;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GNAM")]
        [StringLength(64)]
        public string mGroupName;
        
        [TdfMember("ID")]
        public ulong mId;
        
        [TdfMember("PID")]
        public long mPersonaId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PJID")]
        [StringLength(64)]
        public string mProjectId;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PRID")]
        [StringLength(64)]
        public string mProductId;
        
        [TdfMember("STAT")]
        public EntitlementStatus mStatus;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("TAG")]
        [StringLength(128)]
        public string mEntitlementTag;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("TDAY")]
        [StringLength(32)]
        public string mTerminationDate;
        
        [TdfMember("TYPE")]
        public EntitlementType mEntitlementType;
        
        [TdfMember("UCNT")]
        public uint mUseCount;
        
        [TdfMember("VER")]
        public uint mVersion;
        
        public enum Type : int
        {
            USER = 0x0,
            PERSONA = 0x1,
            DEVICE = 0x2,
        }
        
    }
}
