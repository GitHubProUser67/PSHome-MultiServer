using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubRecord
	{

		[TdfMember("BLID")]
		public long mBlazeId;

		[TdfMember("LUDT")]
		public uint mLastUpdateTime;

		[TdfMember("PERS")]
		public string mPersona;

		[TdfMember("RCDC")]
		public string mRecordDescription;

		[TdfMember("RCID")]
		public uint mRecordId;

		[TdfMember("RCNM")]
		public string mRecordName;

		[TdfMember("STAT")]
		public string mRecordStat;

		[TdfMember("STYP")]
		public RecordStatType mRecordStatType;

	}
}
