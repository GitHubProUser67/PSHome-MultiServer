using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct ResultNotification
    {
        
        [TdfMember("EROR")]
        public int mBlazeError;
        
        [TdfMember("FNL")]
        public bool mFinalResult;
        
        [TdfMember("GRID")]
        public uint mGameReportingId;
        
    }
}
