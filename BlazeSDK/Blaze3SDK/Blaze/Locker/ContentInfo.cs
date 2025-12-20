using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct ContentInfo
	{

		[TdfMember("ATTR")]
		public List<Attribute> mAttributes;

		[TdfMember("BOID")]
		public BlazeObjectId mBlazeObjId;

		[TdfMember("CCAT")]
		public string mContentCategory;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("COID")]
		public long mContextId;

		[TdfMember("CDAT")]
		public int mCreateDate;

		[TdfMember("DFMT")]
		public string mDataFormat;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("EID")]
		public long mEntityId;

		[TdfMember("ENAM")]
		public string mEntityName;

		[TdfMember("EDAT")]
		public int mExpireDate;

		[TdfMember("GURL")]
		public string mGetURL;

		[TdfMember("GPID")]
		public BlazeObjectId mGroupId;

		[TdfMember("URAT")]
		public uint mMyRating;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PERM")]
		public Permission mPermission;

		[TdfMember("RATE")]
		public uint mRate;

		[TdfMember("SIZE")]
		public int mSize;

		[TdfMember("STTS")]
		public Status mStatus;

		[TdfMember("SUBL")]
		public SortedDictionary<string, SubContentInfo> mSubContentInfos;

		[TdfMember("TAGS")]
		public List<Tag> mTags;

		[TdfMember("RCNT")]
		public uint mTotalRatingCount;

		[TdfMember("UDAT")]
		public int mUpdateDate;

		[TdfMember("UURL")]
		public string mUploadURL;

		[TdfMember("UCNT")]
		public int mUseCount;

		[TdfMember("XID")]
		public string mXrefId;

	}
}
