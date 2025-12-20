using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct CreateAccountResponse
	{

		[TdfMember("PNAM")]
		public string mPersonaName;

		[TdfMember("UID")]
		public long mUserId;

	}
}
