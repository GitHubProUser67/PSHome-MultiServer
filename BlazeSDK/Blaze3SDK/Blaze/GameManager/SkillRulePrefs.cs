using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SkillRulePrefs
	{

		[TdfMember("THLD")]
		public string mMinFitThresholdName;

		[TdfMember("SKRN")]
		public string mRuleName;

		[TdfMember("SKDS")]
		public long mSkillValueOverride;

		[TdfMember("SVOR")]
		public SkillValueOverride mUseSkillValueOverride;

	}
}
