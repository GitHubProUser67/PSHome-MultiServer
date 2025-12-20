using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ListContentRequest
	{

		[TdfMember("ATTR")]
		public List<Attribute> mAttributes;

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("EID")]
		public long mEntityId;

		[TdfMember("GPID")]
		public BlazeObjectId mGroupId;

		[TdfMember("PEND")]
		public bool mIncludePending;

		[TdfMember("MXRC")]
		public uint mLimit;

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
