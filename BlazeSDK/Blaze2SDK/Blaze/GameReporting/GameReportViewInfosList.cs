using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReportViewInfosList
    {
        
        [TdfMember("GRPS")]
        public List<GameReportViewInfo> mViewInfo;
        
    }
}
