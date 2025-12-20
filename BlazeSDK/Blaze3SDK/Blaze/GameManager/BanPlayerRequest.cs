using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct BanPlayerRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PLST")]
		public List<long> mPlayerIds;

		[TdfMember("CNTX")]
		public ushort mPlayerRemovedTitleContext;

	}
}
