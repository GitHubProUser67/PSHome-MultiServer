using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardGroupRequest
	{

		[TdfMember("LBID")]
		public int mBoardId;

		[TdfMember("NAME")]
		public string mBoardName;

	}
}
