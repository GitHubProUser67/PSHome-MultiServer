using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct TeamSizeRuleStatus
	{

		[TdfMember("TMAX")]
		public ushort mMaxTeamSizeAccepted;

		[TdfMember("TMIN")]
		public ushort mMinTeamSizeAccepted;

	}
}
