using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct GetContentTopNResponse
	{

		[TdfMember("SIZE")]
		public int mSize;

		[TdfMember("LBRW")]
		public List<LeaderboardValuesRow> mTopNList;

	}
}
