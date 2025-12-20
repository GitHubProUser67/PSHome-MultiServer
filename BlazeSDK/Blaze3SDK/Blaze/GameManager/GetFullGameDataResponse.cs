using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GetFullGameDataResponse
	{

		[TdfMember("LGAM")]
		public List<ListGameData> mGames;

	}
}
