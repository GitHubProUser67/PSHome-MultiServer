using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameSessionUpdatedNotification
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("XNNC")]
		public byte[] mXnetNonce;

		[TdfMember("XSES")]
		public byte[] mXnetSession;

	}
}
