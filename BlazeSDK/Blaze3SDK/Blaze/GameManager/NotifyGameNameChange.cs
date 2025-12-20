using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameNameChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("GNAM")]
		public string mGameName;

	}
}
