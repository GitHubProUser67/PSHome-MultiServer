using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct WipeStatsRequest
	{

		[TdfMember("CAT")]
		public string mCategoryName;

		[TdfMember("EID")]
		public long mEntityId;

		[TdfMember("ETYP")]
		public BlazeObjectId mEntityObjectId;

		[TdfMember("KSUM")]
		public SortedDictionary<string, long> mKeyScopeNameValueMap;

		[TdfMember("OPER")]
		public WipeStatsOperation mOperation;

		public enum WipeStatsOperation : int
		{
			DELETE_BY_CATEGORY_KEYSCOPE_USERSET = 1,
			DELETE_BY_CATEGORY_KEYSCOPE_ENTITYID = 2,
			DELETE_BY_KEYSCOPE_USERSET = 3,
			DELETE_BY_KEYSCOPE_ENTITYID = 4,
			DELETE_BY_ENTITYID = 5,
			DELETE_BY_CATEGORY = 6,
		}

	}
}
