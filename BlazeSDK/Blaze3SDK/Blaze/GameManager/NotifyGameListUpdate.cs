using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameListUpdate
	{

		[TdfMember("DONE")]
		public byte mIsFinalUpdate;

		[TdfMember("GLID")]
		public uint mListId;

		[TdfMember("REMV")]
		public List<uint> mRemovedGameList;

		[TdfMember("UPDT")]
		public List<GameBrowserMatchData> mUpdatedGames;

	}
}
