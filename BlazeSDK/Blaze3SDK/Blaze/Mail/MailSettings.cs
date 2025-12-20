using Tdf;

namespace Blaze3SDK.Blaze.Mail
{
	[TdfStruct]
	public struct MailSettings
	{

		[TdfMember("EFPF")]
		public EmailFormatPref mEmailFormatPref;

		[TdfMember("OPTF")]
		public EmailOptInFlags mEmailOptInFlags;

	}
}
