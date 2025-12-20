using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameSettingsChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("ATTR")]
		public GameSettings mGameSettings;

	}
}
