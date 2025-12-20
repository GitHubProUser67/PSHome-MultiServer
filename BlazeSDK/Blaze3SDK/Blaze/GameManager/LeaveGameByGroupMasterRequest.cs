using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct LeaveGameByGroupMasterRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("CNTX")]
		public ushort mPlayerRemovedTitleContext;

		[TdfMember("SIDL")]
		public List<uint> mSessionIdList;

	}
}
