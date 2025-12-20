using BlazeCommon.PacketDisplayAttributes;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct PingResponse
    {

        [TdfMember("STIM")]
        [DisplayAsDateTime(TimeFormat.UnixSeconds)]
        public uint mServerTime;

    }
}
