using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerListResponse
    {
        
        [TdfMember("LIST")]
        public List<ServerInfoData> mServers;
        
    }
}
