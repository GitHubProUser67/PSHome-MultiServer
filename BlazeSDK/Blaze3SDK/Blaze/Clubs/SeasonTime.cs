using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct SeasonTime
	{

		[TdfMember("SOVR")]
		public int mSeasonRolloverTime;

		[TdfMember("STRT")]
		public int mSeasonStartTime;

	}
}
