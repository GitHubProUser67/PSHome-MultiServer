using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatRowUpdate
	{

		[TdfMember("CAT")]
		public string mCategory;

		[TdfMember("EID")]
		public long mEntityId;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("PTYP")]
		public List<int> mPeriodTypes;

		[TdfMember("UPDT")]
		public List<StatUpdate> mUpdates;

	}
}
