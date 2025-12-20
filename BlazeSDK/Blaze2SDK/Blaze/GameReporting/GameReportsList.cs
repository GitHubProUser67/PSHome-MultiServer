using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReportsList
    {
        
        [TdfMember("GMRS")]
        public List<GameReport> mGameReportList;
        
    }
}
