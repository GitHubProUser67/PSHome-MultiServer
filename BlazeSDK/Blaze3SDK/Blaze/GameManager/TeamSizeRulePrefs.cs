using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct TeamSizeRulePrefs
	{

		[TdfMember("PCNT")]
		public ushort mDesiredPlayerCount;

		[TdfMember("PCAP")]
		public ushort mMaxPlayerCapacity;

		[TdfMember("SDIF")]
		public ushort mMaxTeamSizeDifferenceAllowed;

		[TdfMember("PMIN")]
		public ushort mMinPlayerCount;

		[TdfMember("THLD")]
		public string mRangeOffsetListName;

		[TdfMember("TID")]
		public ushort mTeamId;

		[TdfMember("TLST")]
		public List<ushort> mTeamIdVector;

	}
}
