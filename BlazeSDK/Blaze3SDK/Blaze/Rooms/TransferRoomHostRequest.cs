using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct TransferRoomHostRequest
	{

		[TdfMember("RMID")]
		public uint mRoomId;

		[TdfMember("BZID")]
		public long mUserId;

	}
}
