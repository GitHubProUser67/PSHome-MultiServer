using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct CategorySummary
	{

		[TdfMember("AMSA")]
		public uint mAssociatedMaxSizeAllowed;

		[TdfMember("COTY")]
		public string mContextType;

		[TdfMember("DAFO")]
		public string mDataFormat;

		[TdfMember("ENTY")]
		public string mEntityType;

		[TdfMember("ID")]
		public ushort mId;

		[TdfMember("MAXA")]
		public ushort mMaxAllowed;

		[TdfMember("MBMA")]
		public ushort mMaxBookmarksAllowed;

		[TdfMember("MASA")]
		public uint mMaxSizeAllowed;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("RATE")]
		public bool mRatings;

		[TdfMember("SECU")]
		public bool mSecurePut;

		[TdfMember("TAGS")]
		public bool mTags;

		[TdfMember("USAG")]
		public bool mUsage;

	}
}
