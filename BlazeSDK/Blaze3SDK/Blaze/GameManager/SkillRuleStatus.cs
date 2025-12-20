using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SkillRuleStatus
	{

		[TdfMember("SKMX")]
		public long mMaxSkillAccepted;

		[TdfMember("SKMN")]
		public long mMinSkillAccepted;

		[TdfMember("NAME")]
		public string mRuleName;

	}
}
