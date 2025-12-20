using BlazeCommon.PacketDisplayAttributes;
using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct UserIdentification
    {

        [TdfMember("AID")]
        public long mAccountId;

        [TdfMember("ALOC")]
        [DisplayAsLocale]
        public uint mAccountLocale;

        [TdfMember("EXBB")]
        public byte[] mExternalBlob;

        [TdfMember("EXID")]
        public ulong mExternalId;

        [TdfMember("ID")]
        public uint mBlazeId;

        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(256)]
        public string mName;

        [TdfMember("ONLN")]
        public bool mIsOnline;

        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("PID")]
        [StringLength(32)]
        public string mPersonaId;

    }
}
