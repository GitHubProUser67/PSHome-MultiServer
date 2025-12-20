using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubMember
	{

		[TdfMember("BLID")]
		public long mBlazeId;

		[TdfMember("MSTM")]
		public uint mMembershipSinceTime;

		[TdfMember("CMTP")]
		public MembershipStatus mMembershipStatus;

		[TdfMember("META")]
		public SortedDictionary<string, string> mMetaData;

		[TdfMember("MBOS")]
		public MemberOnlineStatus mOnlineStatus;

		[TdfMember("PERS")]
		public string mPersona;

	}
}
