using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct DeleteUserSettingsRequest
	{

		[TdfMember("KEY")]
		public string mKey;

		[TdfMember("UID")]
		public long mUserId;

	}
}
