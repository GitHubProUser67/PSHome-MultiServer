using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct BannedUserList
	{

		[TdfMember("BLST")]
		public List<long> mBannedList;

	}
}
