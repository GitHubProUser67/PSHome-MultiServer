using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct CreateContentInfoRequest
	{

		[TdfMember("ATTR")]
		public List<Attribute> mAttributes;

		[TdfMember("CCAT")]
		public string mContentCategory;

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

		[TdfMember("SUBL")]
		public List<string> mSubContentNames;

		[TdfMember("TAGS")]
		public List<string> mTags;

	}
}
