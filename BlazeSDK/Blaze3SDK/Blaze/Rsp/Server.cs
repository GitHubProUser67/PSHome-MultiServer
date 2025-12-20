using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct Server
	{

		[TdfMember("BID")]
		public int mBannerId;

		[TdfMember("CRDA")]
		public TimeValue mCreatedDate;

		[TdfMember("EXDA")]
		public TimeValue mExpirationDate;

		[TdfMember("GPVS")]
		public string mGameProtocolVersionString;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("OID")]
		public long mOwnerId;

		[TdfMember("PGID")]
		public string mPersistedGameId;

		[TdfMember("PGSR")]
		public byte[] mPersistedGameIdSecret;

		[TdfMember("PSAL")]
		public string mPingSiteAlias;

		[TdfMember("SID")]
		public uint mServerId;

		[TdfMember("STAT")]
		public ServerStatus mStatus;

		[TdfMember("UPBY")]
		public long mUpdatedBy;

		[TdfMember("UPDA")]
		public TimeValue mUpdatedDate;

	}
}
