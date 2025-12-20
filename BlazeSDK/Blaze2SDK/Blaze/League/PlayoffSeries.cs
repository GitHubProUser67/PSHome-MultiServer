using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct PlayoffSeries
    {
        
        [TdfMember("FORF")]
        public byte mWasWonByForfeit;
        
        [TdfMember("MAXG")]
        public byte mMaxGamesInSeries;
        
        [TdfMember("MUID")]
        public uint mMatchupId;
        
        [TdfMember("P1FS")]
        public byte mPlayer1FinalScore;
        
        [TdfMember("P1ID")]
        public LeagueUser mPlayer1;
        
        [TdfMember("P1TM")]
        public uint mPlayer1TeamId;
        
        [TdfMember("P2FS")]
        public byte mPlayer2FinalScore;
        
        [TdfMember("P2ID")]
        public LeagueUser mPlayer2;
        
        [TdfMember("P2TM")]
        public uint mPlayer2TeamId;
        
        [TdfMember("PLYD")]
        public byte mGamesPlayed;
        
        [TdfMember("PTYP")]
        public PlayoffType mPlayoffType;
        
        [TdfMember("RND")]
        public byte mRound;
        
        [TdfMember("SCRS")]
        public List<PlayoffGameScore> mGameScores;
        
        [TdfMember("WINR")]
        public byte mSeriesWinner;
        
    }
}
