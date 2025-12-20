using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct LeaderboardFolderGroup
	{

		[TdfMember("OWDS")]
		public string mDescription;

		[TdfMember("FLDS")]
		public List<FolderDescriptor> mFolderDescriptors;

		[TdfMember("OWID")]
		public uint mFolderId;

		[TdfMember("META")]
		public string mMetadata;

		[TdfMember("OWNM")]
		public string mName;

		[TdfMember("PRID")]
		public uint mParentId;

		[TdfMember("SDES")]
		public string mShortDesc;

	}
}
