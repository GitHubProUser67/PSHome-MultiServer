using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct RemovePlayerRequest
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("BTPL")]
		public BlazeObjectId mGroupId;

		[TdfMember("PID")]
		public long mPlayerId;

		[TdfMember("REAS")]
		public PlayerRemovedReason mPlayerRemovedReason;

		[TdfMember("CNTX")]
		public ushort mPlayerRemovedTitleContext;

	}
}
