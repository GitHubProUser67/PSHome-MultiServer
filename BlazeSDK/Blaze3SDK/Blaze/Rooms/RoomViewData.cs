using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0x1CAD48C0)]
	public struct RoomViewData
	{

		[TdfMember("META")]
		public SortedDictionary<string, string> mClientMetaData;

		[TdfMember("DISP")]
		public string mDisplayName;

		[TdfMember("GMET")]
		public SortedDictionary<string, string> mGameMetaData;

		[TdfMember("MXRM")]
		public uint mMaxUserRooms;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("USRM")]
		public uint mNumUserRooms;

		[TdfMember("VWID")]
		public uint mViewId;

	}
}
