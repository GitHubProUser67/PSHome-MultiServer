using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardFolderGroupRequest
	{

		[TdfMember("FLID")]
		public uint mFolderId;

		[TdfMember("NAME")]
		public string mFolderName;

	}
}
