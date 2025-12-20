using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct MemberAttributesSet
	{

		[TdfMember("EID")]
		public long mBlazeId;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mMemberAttributes;

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
