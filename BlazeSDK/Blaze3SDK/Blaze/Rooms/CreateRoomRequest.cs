using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct CreateRoomRequest
	{

		[TdfMember("RCAP")]
		public int mCapacity;

		[TdfMember("CTID")]
		public uint mCategoryId;

		[TdfMember("CRIT")]
		public SortedDictionary<string, string> mEntryCriteria;

		[TdfMember("RNAM")]
		public string mName;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mRoomAttributes;

	}
}
