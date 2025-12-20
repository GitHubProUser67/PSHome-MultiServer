using BlazeCommon.PacketDisplayAttributes;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerInstanceInfo
    {

        [TdfMember("ADDR")]
        public ServerAddress mAddress;

        [TdfMember("AMAP")]
        public List<AddressRemapEntry> mAddressRemaps;

        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("MSGS")]
        public List<string> mMessages;

        [TdfMember("NMAP")]
        public List<NameRemapEntry> mNameRemaps;

        [TdfMember("SECU")]
        public bool mSecure;

        [TdfMember("XDNS")]
        [DisplayAsIpAddress]
        public uint mDefaultDnsAddress;

    }
}
