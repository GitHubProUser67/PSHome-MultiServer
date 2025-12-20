using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct CreateCategoryRequest
	{

		[TdfMember("CAPA")]
		public uint mCapacity;

		[TdfMember("CNAM")]
		public string mCatName;

		[TdfMember("CMET")]
		public SortedDictionary<string, string> mClientMetaData;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("DNAM")]
		public string mDisplayName;

		[TdfMember("CRIT")]
		public SortedDictionary<string, string> mEntryCriteria;

		[TdfMember("GMET")]
		public SortedDictionary<string, string> mGameMetaData;

		[TdfMember("JOIN")]
		public bool mJoinIfExists;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mRoomAttributes;

		[TdfMember("VWID")]
		public uint mViewId;

		[TdfMember("VNAM")]
		public string mViewName;

	}
}
