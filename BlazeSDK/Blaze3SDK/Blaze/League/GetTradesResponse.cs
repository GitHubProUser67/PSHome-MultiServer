using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetTradesResponse
	{

		[TdfMember("TRLI")]
		public List<Trade> mTrades;

	}
}
