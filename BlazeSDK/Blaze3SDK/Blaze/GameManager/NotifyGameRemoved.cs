using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameRemoved
	{

		[TdfMember("REAS")]
		public GameDestructionReason mDestructionReason;

		[TdfMember("GID")]
		public uint mGameId;

	}
}
