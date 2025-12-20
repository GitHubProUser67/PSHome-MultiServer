using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameTeamIdChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("NTID")]
		public ushort mNewTeamId;

		[TdfMember("OTID")]
		public ushort mOldTeamId;

		[TdfMember("TIDX")]
		public ushort mTeamIndex;

	}
}
