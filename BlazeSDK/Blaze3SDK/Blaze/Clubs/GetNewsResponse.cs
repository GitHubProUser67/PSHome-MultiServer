using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct GetNewsResponse
	{

		[TdfMember("NWLI")]
		public List<ClubLocalizedNews> mLocalizedNewsList;

		[TdfMember("TLPG")]
		public ushort mTotalPages;

	}
}
