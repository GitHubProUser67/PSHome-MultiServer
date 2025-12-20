using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameBrowserDataList
	{

		[TdfMember("GDAT")]
		public List<GameBrowserGameData> mGameData;

	}
}
