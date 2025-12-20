using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct LookupUsersResponse
	{

		[TdfMember("ULST")]
		public List<UserIdentification> mUserIdentificationList;

	}
}
