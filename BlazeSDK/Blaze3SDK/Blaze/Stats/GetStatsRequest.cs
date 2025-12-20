using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct GetStatsRequest
	{

		[TdfMember("CAT")]
		public string mCategory;

		[TdfMember("EID")]
		public List<long> mEntityIds;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("KSLS")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("POFF")]
		public int mPeriodOffset;

		[TdfMember("PTYP")]
		public int mPeriodType;

		[TdfMember("NAME")]
		public List<string> mStatNames;

	}
}
