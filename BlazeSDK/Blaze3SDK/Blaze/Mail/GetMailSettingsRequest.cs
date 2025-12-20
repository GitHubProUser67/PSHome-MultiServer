using Tdf;

namespace Blaze3SDK.Blaze.Mail
{
	[TdfStruct]
	public struct GetMailSettingsRequest
	{

		[TdfMember("UID")]
		public long mUserId;

	}
}
