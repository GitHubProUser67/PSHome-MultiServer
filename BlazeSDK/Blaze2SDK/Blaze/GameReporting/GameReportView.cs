using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GameReportView
    {
        
        [TdfMember("ENID")]
        public List<uint> mEntityIds;
        
        [TdfMember("LGRC")]
        public List<GameReportColumn> mColumns;
        
    }
}
