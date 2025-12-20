using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0xD27984FB)]
	public struct RoomData
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mAttributes;

		[TdfMember("AREM")]
		public bool mAutoRemove;

		[TdfMember("BLST")]
		public List<long> mBannedUsers;

		[TdfMember("CAP")]
		public int mCapacity;

		[TdfMember("CTID")]
		public uint mCategoryId;

		[TdfMember("CRTM")]
		public uint mCreationTime;

		[TdfMember("CNAM")]
		public string mCreatorPersonaName;

		[TdfMember("CRET")]
		public long mCreatorUserId;

		[TdfMember("CRIT")]
		public SortedDictionary<string, string> mEntryCriteria;

		[TdfMember("HNAM")]
		public string mHostPersonaName;

		[TdfMember("HOST")]
		public long mHostUserId;

		[TdfMember("UCRT")]
		public bool mIsUserCreated;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PSWD")]
		public string mPassword;

		[TdfMember("POPU")]
		public uint mPopulation;

		[TdfMember("PVAL")]
		public string mPseudoValue;

		[TdfMember("RMID")]
		public uint mRoomId;

		[TdfMember("ENUM")]
		public uint mRoomNumber;

	}
}
