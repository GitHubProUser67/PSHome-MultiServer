using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct UpdateMeshConnectionRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("TARG")]
		public List<PlayerConnectionStatus> mMeshConnectionStatusList;

	}
}
