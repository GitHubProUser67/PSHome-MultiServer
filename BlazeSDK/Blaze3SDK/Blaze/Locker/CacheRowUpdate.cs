using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct CacheRowUpdate
	{

		[TdfMember("UPDT")]
		public List<Attribute> mAttributes;

		[TdfMember("CAT")]
		public string mCategory;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("CTYP")]
		public BlazeObjectType mContextType;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

		[TdfMember("HIDE")]
		public bool mHide;

		[TdfMember("TAGS")]
		public List<string> mTags;

	}
}
