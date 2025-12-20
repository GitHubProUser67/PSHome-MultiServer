using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UserData
	{

		[TdfMember("EDAT")]
		public UserSessionExtendedData mExtendedData;

		[TdfMember("FLGS")]
		public UserDataFlags mStatusFlags;

		[TdfMember("USER")]
		public UserIdentification mUserInfo;

	}
}
