using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubRival
	{

		[TdfMember("CRTI")]
		public uint mCreationTime;

		[TdfMember("COP1")]
		public uint mCustOpt1;

		[TdfMember("COP2")]
		public uint mCustOpt2;

		[TdfMember("COP3")]
		public uint mCustOpt3;

		[TdfMember("LATI")]
		public uint mLastUpdateTime;

		[TdfMember("META")]
		public string mMetaData;

		[TdfMember("CLID")]
		public uint mRivalClubId;

	}
}
