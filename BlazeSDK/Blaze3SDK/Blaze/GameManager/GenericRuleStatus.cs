using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GenericRuleStatus
	{

		[TdfMember("VALU")]
		public List<string> mMatchedValues;

		[TdfMember("NAME")]
		public string mRuleName;

	}
}
