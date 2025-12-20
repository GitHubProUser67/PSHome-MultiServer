using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SetPlayerTeamRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PID")]
		public long mPlayerId;

		[TdfMember("TIDX")]
		public ushort mTeamIndex;

	}
}
