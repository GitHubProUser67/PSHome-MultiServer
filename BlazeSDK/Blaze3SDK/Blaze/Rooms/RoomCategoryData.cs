using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0xBFED2619)]
	public struct RoomCategoryData
	{

		[TdfMember("CAPA")]
		public uint mCapacity;

		[TdfMember("CTID")]
		public uint mCategoryId;

		[TdfMember("CMET")]
		public SortedDictionary<string, string> mClientMetaData;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("DISP")]
		public string mDisplayName;

		[TdfMember("CRIT")]
		public SortedDictionary<string, string> mEntryCriteria;

		[TdfMember("EPCT")]
		public ushort mExpandThresholdPercent;

		[TdfMember("FLAG")]
		public RoomCategoryFlags mFlags;

		[TdfMember("GMET")]
		public SortedDictionary<string, string> mGameMetaData;

		[TdfMember("UCRT")]
		public bool mIsUserCreated;

		[TdfMember("LOCL")]
		public string mLocale;

		[TdfMember("EMAX")]
		public uint mMaxExpandedRooms;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("NEXP")]
		public ushort mNumExpandedRooms;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("DISR")]
		public string mRoomDisplayName;

		[TdfMember("VWID")]
		public uint mViewId;

	}
}
