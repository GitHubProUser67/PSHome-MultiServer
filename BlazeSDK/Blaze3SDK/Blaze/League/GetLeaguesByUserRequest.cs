using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetLeaguesByUserRequest
	{

		[TdfMember("ONLN")]
		public byte mFindNumberOfMembersOnline;

	}
}
