using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct VirtualGameRulePrefs
	{

		[TdfMember("VALU")]
		public VirtualGameDesiredValue mDesiredVirtualGameValue;

		[TdfMember("THLD")]
		public string mMinFitThresholdName;

		public enum VirtualGameDesiredValue : int
		{
			STANDARD = 1,
			VIRTUALIZED = 2,
			ABSTAIN = 8,
		}

	}
}
