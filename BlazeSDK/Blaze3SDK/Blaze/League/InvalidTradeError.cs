using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct InvalidTradeError
	{

		[TdfMember("RESN")]
		public int mReasonCode;

	}
}
