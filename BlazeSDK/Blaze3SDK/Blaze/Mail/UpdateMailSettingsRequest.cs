using Tdf;

namespace Blaze3SDK.Blaze.Mail
{
	[TdfStruct]
	public struct UpdateMailSettingsRequest
	{

		[TdfMember("MSET")]
		public MailSettings mMailSettings;

	}
}
