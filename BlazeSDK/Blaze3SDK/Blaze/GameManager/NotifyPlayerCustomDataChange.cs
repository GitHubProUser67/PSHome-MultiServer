using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyPlayerCustomDataChange
	{

		[TdfMember("CDAT")]
		public byte[] mCustomData;

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("PID")]
		public long mPlayerId;

	}
}
