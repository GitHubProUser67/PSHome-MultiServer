using Tdf;

namespace Blaze3SDK.Blaze.Mail
{
	[TdfStruct]
	public struct SetMailOptInFlagsRequest
	{

		[TdfMember("OPTF")]
		public EmailOptInFlags mEmailOptInFlags;

		[TdfMember("UID")]
		public long mUserId;

	}
}
