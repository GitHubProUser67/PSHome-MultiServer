using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct HostViabilityRuleStatus
	{

		[TdfMember("VVAL")]
		public HostViabilityValues mMatchedHostViabilityValue;

		public enum HostViabilityValues : int
		{
			CONNECTION_ASSURED = 0,
			CONNECTION_LIKELY = 1,
			CONNECTION_FEASIBLE = 2,
			CONNECTION_UNLIKELY = 3,
		}

	}
}
