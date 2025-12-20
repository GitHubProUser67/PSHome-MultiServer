using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct UEDRuleStatus
	{

		[TdfMember("AMAX")]
		public long mMaxUEDAccepted;

		[TdfMember("AMIN")]
		public long mMinUEDAccepted;

		[TdfMember("MUED")]
		public long mMyUEDValue;

		[TdfMember("NAME")]
		public string mRuleName;

	}
}
