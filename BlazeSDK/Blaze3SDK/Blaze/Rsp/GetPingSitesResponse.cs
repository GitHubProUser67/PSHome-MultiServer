using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct GetPingSitesResponse
	{

		[TdfMember("PLST")]
		public List<RspPingSiteInfo> mPingSites;

	}
}
