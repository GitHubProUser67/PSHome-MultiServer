using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameBrowserPlayerData
	{

		[TdfMember("LOC")]
		public uint mAccountLocale;

		[TdfMember("EXID")]
		public ulong mExternalId;

		[TdfMember("PATT")]
		public SortedDictionary<string, string> mPlayerAttribs;

		[TdfMember("PID")]
		public long mPlayerId;

		[TdfMember("NAME")]
		public string mPlayerName;

		[TdfMember("STAT")]
		public PlayerState mPlayerState;

		[TdfMember("TIDX")]
		public ushort mTeamIndex;

	}
}
