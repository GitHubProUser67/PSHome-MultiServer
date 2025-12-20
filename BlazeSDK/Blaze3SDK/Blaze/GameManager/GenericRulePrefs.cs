using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GenericRulePrefs
	{

		[TdfMember("VALU")]
		public List<string> mDesiredValues;

		[TdfMember("THLD")]
		public string mMinFitThresholdName;

		[TdfMember("NAME")]
		public string mRuleName;

	}
}
