using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GetGameListRequest
	{

		[TdfMember("GVER")]
		public string mGameProtocolVersionString;

		[TdfMember("IGNO")]
		public bool mIgnoreGameEntryCriteria;

		[TdfMember("NOJM")]
		public bool mIgnoreGameJoinMethod;

		[TdfMember("LCAP")]
		public uint mListCapacity;

		[TdfMember("DNAM")]
		public string mListConfigName;

		[TdfMember("GLID")]
		public MatchmakingCriteriaData mListCriteria;

	}
}
