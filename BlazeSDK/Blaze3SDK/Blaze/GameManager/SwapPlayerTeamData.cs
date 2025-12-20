using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SwapPlayerTeamData
	{

		[TdfMember("PID")]
		public long mPlayerId;

		[TdfMember("TIDX")]
		public ushort mTeamIndex;

	}
}
