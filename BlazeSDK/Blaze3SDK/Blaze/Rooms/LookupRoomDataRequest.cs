using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct LookupRoomDataRequest
	{

		[TdfMember("RMID")]
		public List<uint> mRoomIdList;

	}
}
