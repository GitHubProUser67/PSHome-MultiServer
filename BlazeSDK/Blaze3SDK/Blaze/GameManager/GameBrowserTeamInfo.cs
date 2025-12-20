using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameBrowserTeamInfo
	{

		[TdfMember("TID")]
		public ushort mTeamId;

		[TdfMember("TSZE")]
		public ushort mTeamSize;

	}
}
