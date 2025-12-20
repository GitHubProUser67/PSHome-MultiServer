using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct RankedGameRulePrefs
	{

		[TdfMember("VALU")]
		public RankedGameDesiredValue mDesiredRankedGameValue;

		[TdfMember("THLD")]
		public string mMinFitThresholdName;

		public enum RankedGameDesiredValue : int
		{
			UNRANKED = 1,
			RANKED = 2,
			RANDOM = 4,
			ABSTAIN = 8,
		}

	}
}
