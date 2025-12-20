using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct TournamentTrophyData
	{

		[TdfMember("CONT")]
		public uint mAwardCount;

		[TdfMember("TIME")]
		public uint mAwardTime;

		[TdfMember("TNID")]
		public uint mTournamentId;

		[TdfMember("META")]
		public string mTrophyMetaData;

		[TdfMember("TNAM")]
		public string mTrophyName;

		[TdfMember("TLOC")]
		public string mTrophyNameLocKey;

		[TdfMember("BZID")]
		public long mUserId;

	}
}
