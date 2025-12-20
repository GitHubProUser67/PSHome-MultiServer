using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct LoginRequest
	{

		[TdfMember("DVID")]
		public ulong mDeviceId;

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("TOKN")]
		public string mToken;

		[TdfMember("TYPE")]
		public TOKENTYPE mTokenType;

	}
}
