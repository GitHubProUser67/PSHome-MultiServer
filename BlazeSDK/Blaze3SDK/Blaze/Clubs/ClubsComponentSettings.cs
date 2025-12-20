using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubsComponentSettings
	{

		[TdfMember("AWST")]
		public List<AwardSettings> mAwardSettings;

		[TdfMember("CLDS")]
		public ushort mClubDivisionSize;

		[TdfMember("DMNS")]
		public List<ClubDomain> mDomainList;

		[TdfMember("MXEV")]
		public ushort mMaxEvents;

		[TdfMember("MXRV")]
		public ushort mMaxRivalsPerClub;

		[TdfMember("PUHR")]
		public ushort mPurgeHour;

		[TdfMember("REST")]
		public List<RecordSettings> mRecordSettings;

		[TdfMember("SOVR")]
		public int mSeasonRolloverTime;

		[TdfMember("STRT")]
		public int mSeasonStartTime;

	}
}
