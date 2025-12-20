using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ListDeviceAccountsResponse
	{

		[TdfMember("USRL")]
		public List<UserDetails> mUserList;

	}
}
