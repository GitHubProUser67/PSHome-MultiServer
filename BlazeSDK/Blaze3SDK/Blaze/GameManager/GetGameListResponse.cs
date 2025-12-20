using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GetGameListResponse
	{

		[TdfMember("GLID")]
		public uint mListId;

		[TdfMember("MAXF")]
		public uint mMaxPossibleFitScore;

		[TdfMember("NGD")]
		public uint mNumberOfGamesToBeDownloaded;

	}
}
