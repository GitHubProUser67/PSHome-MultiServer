using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct PostAuthResponse
    {
        
        [TdfMember("TELE")]
        public GetTelemetryServerResponse mTelemetryServer;
        
        [TdfMember("TICK")]
        public GetTickerServerResponse mTickerServer;
        
    }
}
