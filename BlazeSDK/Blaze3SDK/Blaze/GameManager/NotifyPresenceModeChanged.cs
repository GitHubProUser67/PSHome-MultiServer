using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyPresenceModeChanged
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PRES")]
		public PresenceMode mNewPresenceMode;

	}
}
