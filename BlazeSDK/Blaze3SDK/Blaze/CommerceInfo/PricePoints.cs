using Tdf;

namespace Blaze3SDK.Blaze.CommerceInfo
{
	[TdfStruct]
	public struct PricePoints
	{

		[TdfMember("TCTE")]
		public bool mIsFree;

		[TdfMember("PPL")]
		public List<PricePoint> mPricePointVector;

	}
}
