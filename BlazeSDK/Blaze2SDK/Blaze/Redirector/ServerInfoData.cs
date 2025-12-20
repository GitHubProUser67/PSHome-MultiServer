using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerInfoData
    {
        
        [TdfMember("AMAP")]
        public List<AddressRemapEntry> mAddressRemaps;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("BTIM")]
        [StringLength(256)]
        public string mBuildTime;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("CVER")]
        public List<string> mCompatibleClientVersions;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("DEPO")]
        [StringLength(256)]
        public string mDepotLocation;
        
        [TdfMember("INST")]
        public List<ServerInstance> mInstances;
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("IVER")]
        public List<string> mIncompatibleClientVersions;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("LOCN")]
        [StringLength(256)]
        public string mBuildLocation;
        
        [TdfMember("MSTR")]
        public ServerInstance mMasterInstance;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mName;
        
        [TdfMember("NMAP")]
        public List<NameRemapEntry> mNameRemaps;
        
        [TdfMember("SVID")]
        public uint mDefaultServiceId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("VERS")]
        [StringLength(256)]
        public string mVersion;
        
        [TdfMember("XDNS")]
        public uint mDefaultDnsAddress;
        
        [TdfMember("XMST")]
        public List<ServerInstance> mAuxMasters;
        
    }
}
