using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct DestroyGameRequest
	{

		[TdfMember("REAS")]
		public GameDestructionReason mDestructionReason;

		[TdfMember("GID")]
		public uint mGameId;

	}
}
