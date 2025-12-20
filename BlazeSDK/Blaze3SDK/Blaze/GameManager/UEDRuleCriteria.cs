using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct UEDRuleCriteria
	{

		[TdfMember("CVAL")]
		public long mClientUEDSearchValue;

		[TdfMember("OVAL")]
		public long mOverrideUEDValue;

		[TdfMember("NAME")]
		public string mRuleName;

		[TdfMember("THLD")]
		public string mThresholdName;

	}
}
