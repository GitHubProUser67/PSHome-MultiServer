using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ListContentForUsersRequest
	{

		[TdfMember("ATTR")]
		public List<Attribute> mAttributes;

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("EID")]
		public List<long> mEntityIds;

		[TdfMember("GPID")]
		public BlazeObjectId mGroupId;

		[TdfMember("MXRC")]
		public uint mMaxResultCount;

		[TdfMember("OJID")]
		public BlazeObjectId mObjectId;

		[TdfMember("OFRC")]
		public uint mOffset;

		[TdfMember("PERM")]
		public PermissionFlag mPermissionFlag;

		[TdfMember("REFF")]
		public RefSearchFlag mReferenceFlag;

		[TdfMember("TAGS")]
		public List<Tag> mTags;

	}
}
