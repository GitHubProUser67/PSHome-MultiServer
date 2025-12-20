using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GetGameDataFromIdRequest
	{

		[TdfMember("GLST")]
		public List<uint> mGameIds;

		[TdfMember("DNAM")]
		public string mListConfigName;

		[TdfMember("PIDL")]
		public List<string> mPersistedGameIdList;

	}
}
