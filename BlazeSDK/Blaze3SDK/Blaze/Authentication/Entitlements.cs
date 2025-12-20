using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct Entitlements
	{

		[TdfMember("NLST")]
		public List<Entitlement> mEntitlements;

	}
}
