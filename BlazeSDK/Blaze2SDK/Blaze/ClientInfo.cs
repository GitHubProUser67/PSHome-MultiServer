using BlazeCommon.PacketDisplayAttributes;
using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct ClientInfo
    {

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("BSDK")]
        [StringLength(64)]
        public string mBlazeSDKVersion;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("BTIM")]
        [StringLength(64)]
        public string mBlazeSDKBuildDate;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("CLNT")]
        [StringLength(64)]
        public string mClientName;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("CSKU")]
        [StringLength(64)]
        public string mClientSkuId;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("CVER")]
        [StringLength(64)]
        public string mClientVersion;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("DSDK")]
        [StringLength(64)]
        public string mDirtySDKVersion;

        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("ENV")]
        [StringLength(16)]
        public string mEnvironment;

        [TdfMember("LOC")]
        [DisplayAsLocale]
        public uint mClientLocale;

        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("MAC")]
        [StringLength(32)]
        public string mMacAddress;

        /// <summary>
        /// Max String Length: 16
        /// </summary>
        [TdfMember("PLAT")]
        [StringLength(16)]
        public string mPlatform;

    }
}
