using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct BannedListResponse
	{

		[TdfMember("BANM")]
		public List<long> mBannedMembers;

	}
}
