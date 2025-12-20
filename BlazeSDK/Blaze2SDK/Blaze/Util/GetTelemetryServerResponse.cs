using BlazeCommon.PacketDisplayAttributes;
using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct GetTelemetryServerResponse
    {

        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("ADRS")]
        [StringLength(64)]
        public string mAddress;

        [TdfMember("ANON")]
        public bool mIsAnonymous;

        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("DISA")]
        [StringLength(1024)]
        public string mDisable;

        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("FILT")]
        [StringLength(1024)]
        public string mFilter;

        [TdfMember("LOC")]
        [DisplayAsLocale]
        public uint mLocale;

        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("NOOK")]
        [StringLength(1024)]
        public string mNoToggleOk;

        [TdfMember("PORT")]
        public uint mPort;

        [TdfMember("SDLY")]
        public uint mSendDelay;

        /// <summary>
        /// Max String Length: 512
        /// </summary>
        [TdfMember("SKEY")]
        [StringLength(512)]
        public string mKey;

        [TdfMember("SPCT")]
        public uint mSendPercentage;

    }
}
