using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardViewResponse
	{

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CTYP")]
		public BlazeObjectType mContextType;

		[TdfMember("DESC")]
		public string mDesc;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("LB")]
		public LeaderboardType mLeaderboardType;

		[TdfMember("VIEW")]
		public string mLeaderboardView;

		[TdfMember("SIZE")]
		public int mSize;

		[TdfMember("TAGS")]
		public int mTagsIncluded;

		[TdfMember("LIST")]
		public List<LeaderboardViewColumn> mViewColumns;

	}
}
