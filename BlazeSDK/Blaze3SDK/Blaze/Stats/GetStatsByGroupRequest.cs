using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct GetStatsByGroupRequest
	{

		[TdfMember("AGGR")]
		public AggregateCalcFlags mAggrFlags;

		[TdfMember("EID")]
		public List<long> mEntityIds;

		[TdfMember("NAME")]
		public string mGroupName;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("PCTR")]
		public int mPeriodCtr;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("PTYP")]
		public int mPeriodType;

		[TdfMember("TIME")]
		public int mTime;

		[TdfMember("VID")]
		public uint mViewId;

	}
}
