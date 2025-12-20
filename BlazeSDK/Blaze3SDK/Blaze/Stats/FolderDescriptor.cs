using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct FolderDescriptor
	{

		[TdfMember("FLDS")]
		public string mDescription;

		[TdfMember("FLID")]
		public uint mFolderId;

		[TdfMember("FLNM")]
		public string mName;

		[TdfMember("SDES")]
		public string mShortDesc;

	}
}
