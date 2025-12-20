using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct SetRoomAttributesRequest
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mRoomAttributes;

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
