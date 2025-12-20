using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubGameSummary
	{

		[TdfMember("NLUP")]
		public uint mLastUpdateTime;

		[TdfMember("NLOS")]
		public uint mLoss;

		[TdfMember("OPID")]
		public uint mOppoClubId;

		[TdfMember("NTIE")]
		public uint mTie;

		[TdfMember("NWIN")]
		public uint mWin;

	}
}
