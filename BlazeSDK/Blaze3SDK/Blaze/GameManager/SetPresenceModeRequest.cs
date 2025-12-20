using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SetPresenceModeRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PRES")]
		public PresenceMode mPresenceMode;

	}
}
