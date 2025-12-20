using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameSizeRuleStatus
	{

		[TdfMember("PMAX")]
		public uint mMaxPlayerCountAccepted;

		[TdfMember("PMIN")]
		public uint mMinPlayerCountAccepted;

	}
}
