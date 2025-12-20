using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct HostBalanceRuleStatus
	{

		[TdfMember("BVAL")]
		public HostBalanceValues mMatchedHostBalanceValue;

		public enum HostBalanceValues : int
		{
			HOSTS_STRICTLY_BALANCED = 0,
			HOSTS_BALANCED = 1,
			HOSTS_UNBALANCED = 2,
		}

	}
}
