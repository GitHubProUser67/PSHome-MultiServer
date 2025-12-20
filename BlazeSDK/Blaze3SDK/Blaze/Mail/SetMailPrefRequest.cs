using Tdf;

namespace Blaze3SDK.Blaze.Mail
{
	[TdfStruct]
	public struct SetMailPrefRequest
	{

		[TdfMember("EFPF")]
		public EmailFormatPref mEmailFormatPref;

		[TdfMember("UID")]
		public long mUserId;

	}
}
