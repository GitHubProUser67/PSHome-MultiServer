using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct(0xE027A5F5)]
	public struct RoomMemberData
	{

		[TdfMember("BZID")]
		public long mBlazeId;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mMemberAttributes;

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
