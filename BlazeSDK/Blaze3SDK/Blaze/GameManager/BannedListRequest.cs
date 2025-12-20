using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct BannedListRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

	}
}
