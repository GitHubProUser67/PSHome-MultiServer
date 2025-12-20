using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct NotifyUserAdded
	{

		[TdfMember("DATA")]
		public UserSessionExtendedData mExtendedData;

		[TdfMember("USER")]
		public UserIdentification mUserInfo;

	}
}
