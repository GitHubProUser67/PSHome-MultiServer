using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SwapPlayersTeamRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("LGAM")]
		public List<SwapPlayerTeamData> mSwapPlayersTeam;

	}
}
