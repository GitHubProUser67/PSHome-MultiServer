using Blaze2SDK.Blaze.Util;
using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct UserSessionExtendedData
    {

        [TdfMember("ADDR")]
        public NetworkAddress mAddress;

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("BPS")]
        [StringLength(64)]
        public string mBestPingSiteAlias;

        [TdfMember("CMAP")]
        public SortedDictionary<uint, int> mClientAttributes;

        /// <summary>
        /// Max String Length: 4
        /// </summary>
        [TdfMember("CTY")]
        [StringLength(4)]
        public string mCountry;

        [TdfMember("DMAP")]
        public SortedDictionary<uint, int> mDataMap;

        [TdfMember("HWFG")]
        public HardwareFlags mHardwareFlags;

        [TdfMember("PSLM")]
        public List<int> mLatencyList;

        [TdfMember("QDAT")]
        public NetworkQosData mQosData;

        [TdfMember("UATT")]
        public ulong mUserInfoAttribute;

        [TdfMember("ULST")]
        public List<ulong> mBlazeObjectIdList;

    }
}
