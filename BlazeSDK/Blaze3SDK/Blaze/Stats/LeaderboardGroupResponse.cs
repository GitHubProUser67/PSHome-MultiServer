using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardGroupResponse
	{

		[TdfMember("ASCD")]
		public bool mAscending;

		[TdfMember("BNAM")]
		public string mBoardName;

		[TdfMember("DESC")]
		public string mDesc;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("KSUM")]
		public SortedDictionary<string, ScopeValues> mKeyScopeNameValueListMap;

		[TdfMember("LBSZ")]
		public int mLeaderboardSize;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("LIST")]
		public List<StatDescSummary> mStatKeyColumns;

		[TdfMember("SNAM")]
		public string mStatName;

	}
}
