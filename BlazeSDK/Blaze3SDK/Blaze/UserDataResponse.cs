using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UserDataResponse
	{

		[TdfMember("ULST")]
		public List<UserData> mUserDataList;

	}
}
