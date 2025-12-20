using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardStatValues
	{

		[TdfMember("LDLS")]
		public List<LeaderboardStatValuesRow> mRows;

	}
}
