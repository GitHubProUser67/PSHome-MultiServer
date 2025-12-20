using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct UserSettingsSaveRequest
	{

		[TdfMember("DATA")]
		public string mData;

		[TdfMember("KEY")]
		public string mKey;

		[TdfMember("UID")]
		public long mUserId;

	}
}
