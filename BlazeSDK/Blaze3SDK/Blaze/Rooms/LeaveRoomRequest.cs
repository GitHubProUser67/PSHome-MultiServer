using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct LeaveRoomRequest
	{

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
