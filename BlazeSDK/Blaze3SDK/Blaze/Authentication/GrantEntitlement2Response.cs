using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct GrantEntitlement2Response
	{

		[TdfMember("ENTI")]
		public Entitlement mEntitlementInfo;

		[TdfMember("ISGR")]
		public bool mIsGranted;

	}
}
