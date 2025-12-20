using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyGameCapacityChange
	{

		[TdfMember("GID")]
		public uint mGameId;

		[TdfMember("CAP")]
		public List<ushort> mSlotCapacities;

		[TdfMember("TCAP")]
		public ushort mTeamCapacity;

	}
}
