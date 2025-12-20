using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct UpdateContentInfoRequest
	{

		[TdfMember("ATTR")]
		public List<Attribute> mAttributes;

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("COID")]
		public long mContextId;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("EDAT")]
		public int mExpireDate;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PERM")]
		public Permission mPermission;

		[TdfMember("SIZE")]
		public uint mSize;

		[TdfMember("TAGS")]
		public List<string> mTags;

	}
}
