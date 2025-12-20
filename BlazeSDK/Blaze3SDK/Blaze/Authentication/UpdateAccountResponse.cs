using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct UpdateAccountResponse
	{

		[TdfMember("PCTK")]
		public string mPCLoginToken;

	}
}
