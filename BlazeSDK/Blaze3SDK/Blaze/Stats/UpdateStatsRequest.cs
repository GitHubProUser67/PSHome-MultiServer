using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct UpdateStatsRequest
	{

		[TdfMember("UPDT")]
		public List<StatRowUpdate> mStatUpdates;

	}
}
