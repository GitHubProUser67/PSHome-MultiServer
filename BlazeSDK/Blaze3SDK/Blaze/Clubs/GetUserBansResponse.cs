using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetUserBansResponse
	{

		[TdfMember("BANS")]
		public SortedDictionary<uint, uint> mClubIdToBanStatusMap;

	}
}
