using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct ListGamesResponse
	{

		[TdfMember("LGAM")]
		public List<ListGameData> mGames;

	}
}
