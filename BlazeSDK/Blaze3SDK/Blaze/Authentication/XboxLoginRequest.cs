using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct XboxLoginRequest
	{

		[TdfMember("MAIL")]
		public string mEmail;

		[TdfMember("GTAG")]
		public string mGamerTag;

		[TdfMember("XUID")]
		public ulong mXuid;

	}
}
