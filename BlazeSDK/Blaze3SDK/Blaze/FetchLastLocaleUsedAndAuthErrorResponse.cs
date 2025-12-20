using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct FetchLastLocaleUsedAndAuthErrorResponse
	{

		[TdfMember("LAHE")]
		public string mLastAuthError;

		[TdfMember("LLUD")]
		public string mLastLocaleUsed;

	}
}
