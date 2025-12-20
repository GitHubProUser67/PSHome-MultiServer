using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardStatValuesRow
	{

		[TdfMember("UATT")]
		public ulong mAttribute;

		[TdfMember("ENID")]
		public long mEntityId;

		[TdfMember("ENAM")]
		public string mEntityName;

		[TdfMember("RWFG")]
		public bool mIsRawStats;

		[TdfMember("RWOT")]
		public List<StatRawValue> mOtherRawStats;

		[TdfMember("STAT")]
		public List<string> mOtherStats;

		[TdfMember("RANK")]
		public int mRank;

		[TdfMember("RWST")]
		public StatRawValue mRankedRawStat;

		[TdfMember("RSTA")]
		public string mRankedStat;

	}
}
