using Tdf;

namespace Blaze3SDK.Blaze.Mail
{
	[TdfStruct]
	public struct GetMailSettingsResponse
	{

		[TdfMember("MSET")]
		public MailSettings mMailSettings;

	}
}
