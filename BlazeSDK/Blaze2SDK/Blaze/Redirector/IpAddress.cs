using BlazeCommon.PacketDisplayAttributes;
using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct IpAddress
    {

        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("HOST")]
        [StringLength(128)]
        public string mHostname;

        [TdfMember("IP")]
        [DisplayAsIpAddress]
        public uint mIp;

        [TdfMember("PORT")]
        public ushort mPort;

    }
}
