using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetPetitionsResponse
	{

		[TdfMember("CIST")]
		public List<ClubMessage> mClubPetitionsList;

	}
}
