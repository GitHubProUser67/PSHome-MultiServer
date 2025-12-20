using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct PlayoffSeries
	{

		[TdfMember("SCRS")]
		public List<PlayoffGameScore> mGameScores;

		[TdfMember("PLYD")]
		public byte mGamesPlayed;

		[TdfMember("MUID")]
		public uint mMatchupId;

		[TdfMember("MAXG")]
		public byte mMaxGamesInSeries;

		[TdfMember("P1ID")]
		public LeagueUser mPlayer1;

		[TdfMember("P1FS")]
		public byte mPlayer1FinalScore;

		[TdfMember("P1TM")]
		public uint mPlayer1TeamId;

		[TdfMember("P2ID")]
		public LeagueUser mPlayer2;

		[TdfMember("P2FS")]
		public byte mPlayer2FinalScore;

		[TdfMember("P2TM")]
		public uint mPlayer2TeamId;

		[TdfMember("PTYP")]
		public PlayoffType mPlayoffType;

		[TdfMember("RND")]
		public byte mRound;

		[TdfMember("WINR")]
		public byte mSeriesWinner;

		[TdfMember("FORF")]
		public byte mWasWonByForfeit;

	}
}
