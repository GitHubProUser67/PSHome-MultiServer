using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetClubBansResponse
	{

		[TdfMember("BANS")]
		public SortedDictionary<long, uint> mUserIdToBanStatusMap;

	}
}
