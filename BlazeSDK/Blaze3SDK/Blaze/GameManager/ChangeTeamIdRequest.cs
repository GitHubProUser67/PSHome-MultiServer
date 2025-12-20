using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct ChangeTeamIdRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("NTID")]
		public ushort mNewTeamId;

		[TdfMember("TIDX")]
		public ushort mTeamIndex;

	}
}
