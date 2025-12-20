using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct DNFRuleStatus
	{

		[TdfMember("XDNF")]
		public long mMaxDNFValue;

		[TdfMember("MDNF")]
		public long mMyDNFValue;

	}
}
