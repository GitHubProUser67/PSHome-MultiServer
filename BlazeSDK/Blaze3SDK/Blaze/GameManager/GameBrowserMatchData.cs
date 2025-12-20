using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameBrowserMatchData
	{

		[TdfMember("FIT")]
		public uint mFitScore;

		[TdfMember("GAM")]
		public GameBrowserGameData mGameData;

	}
}
