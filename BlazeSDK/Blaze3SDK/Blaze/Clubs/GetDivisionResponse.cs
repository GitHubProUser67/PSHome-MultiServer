using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetDivisionResponse
	{

		[TdfMember("DIVN")]
		public uint mDivision;

		[TdfMember("SRNK")]
		public uint mStartingRank;

	}
}
