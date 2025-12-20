using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct UserSettingsLoadRequest
	{

		[TdfMember("KEY")]
		public string mKey;

		[TdfMember("UID")]
		public long mUserId;

	}
}
