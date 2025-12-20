using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct SessionInfo
	{

		[TdfMember("BUID")]
		public long mBlazeUserId;

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("FRST")]
		public bool mIsFirstLogin;

		[TdfMember("LLOG")]
		public long mLastLoginDateTime;

		[TdfMember("PDTL")]
		public PersonaDetails mPersonaDetails;

		[TdfMember("KEY")]
		public string mSessionKey;

		[TdfMember("UID")]
		public long mUserId;

	}
}
