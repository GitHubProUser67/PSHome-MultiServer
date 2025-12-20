using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetClubAwardsResponse
	{

		[TdfMember("AWRL")]
		public List<ClubAward> mClubAwardList;

	}
}
