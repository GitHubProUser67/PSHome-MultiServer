using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct SubmitStatisticsRequest
	{

		[TdfMember("CATE")]
		public string mCategory;

		[TdfMember("ENTS")]
		public List<uint> mEntities;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("NENT")]
		public int mNumEntities;

		[TdfMember("NSTA")]
		public int mNumStats;

		[TdfMember("STAT")]
		public List<int> mStats;

	}
}
