using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatGroupResponse
	{

		[TdfMember("CNAM")]
		public string mCategoryName;

		[TdfMember("DESC")]
		public string mDesc;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("STAT")]
		public List<StatDescSummary> mStatDescs;

	}
}
