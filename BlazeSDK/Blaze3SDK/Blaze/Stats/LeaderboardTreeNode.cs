using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardTreeNode
	{

		[TdfMember("CHDS")]
		public uint mFirstChild;

		[TdfMember("LAST")]
		public bool mLastNode;

		[TdfMember("CHDE")]
		public uint mNext2Last;

		[TdfMember("NDID")]
		public uint mNodeId;

		[TdfMember("NAME")]
		public string mNodeName;

		[TdfMember("RTNM")]
		public string mRootName;

		[TdfMember("SDES")]
		public string mShortDesc;

	}
}
