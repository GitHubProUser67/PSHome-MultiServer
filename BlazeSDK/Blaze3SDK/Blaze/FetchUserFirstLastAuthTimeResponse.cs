using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct FetchUserFirstLastAuthTimeResponse
	{

		[TdfMember("UFAT")]
		public long mUserFirstAuthTime;

		[TdfMember("ULAT")]
		public long mUserLastAuthTime;

	}
}
