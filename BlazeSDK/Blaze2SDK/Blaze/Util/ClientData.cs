using BlazeCommon.PacketDisplayAttributes;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct ClientData
    {

        [TdfMember("LANG")]
        [DisplayAsLocale]
        public uint mLocale;

        [TdfMember("TYPE")]
        public ClientType mClientType;

    }
}
