using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct BanPlayerMasterRequest
	{

		[TdfMember("ALST")]
		public List<long> mAccountIds;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PLST")]
		public List<long> mPlayerIds;

		[TdfMember("CNTX")]
		public ushort mPlayerRemovedTitleContext;

	}
}
