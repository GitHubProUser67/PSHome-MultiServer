using BlazeCommon.PacketDisplayAttributes;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct IpAddress
    {

        [TdfMember("IP")]
        [DisplayAsIpAddress]
        public uint Ip;

        [TdfMember("PORT")]
        public ushort Port;

    }
}
