using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct CacheDelete
	{

		[TdfMember("CAT")]
		public string mCategory;

		[TdfMember("CID")]
		public int mContentId;

		[TdfMember("CTYP")]
		public BlazeObjectType mContextType;

		[TdfMember("ETYP")]
		public BlazeObjectType mEntityType;

	}
}
