using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ListRivalsResponse
	{

		[TdfMember("RIVL")]
		public List<ClubRival> mClubRivalList;

	}
}
