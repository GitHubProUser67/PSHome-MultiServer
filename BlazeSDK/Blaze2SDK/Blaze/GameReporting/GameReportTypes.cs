using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReportTypes
    {
        
        [TdfMember("GRTS")]
        public List<GameReportType> mGameReportTypes;
        
    }
}
