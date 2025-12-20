using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct StatCategorySummary
	{

		[TdfMember("CTYP")]
		public CategoryType mCategoryType;

		[TdfMember("DESC")]
		public string mDesc;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("KEYS")]
		public List<string> mKeyScopes;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PTYP")]
		public List<StatPeriodType> mPeriodTypes;

	}
}
