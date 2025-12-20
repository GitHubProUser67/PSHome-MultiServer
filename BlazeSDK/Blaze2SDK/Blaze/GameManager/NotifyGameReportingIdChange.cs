using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct NotifyGameReportingIdChange
    {
        
        [TdfMember("GID")]
        public uint gameId;
        
        [TdfMember("GRID")]
        public uint gameReportingId;
        
    }
}
