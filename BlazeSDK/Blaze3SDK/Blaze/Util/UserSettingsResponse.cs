using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct UserSettingsResponse
	{

		[TdfMember("DATA")]
		public string mData;

		[TdfMember("KEY")]
		public string mKey;

	}
}
