using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubInfo
	{

		[TdfMember("AWCN")]
		public uint mAwardCount;

		[TdfMember("CRTI")]
		public uint mCreationTime;

		[TdfMember("GMCN")]
		public uint mGmCount;

		[TdfMember("LATI")]
		public uint mLastActiveTime;

		[TdfMember("LSGR")]
		public string mLastGameResult;

		[TdfMember("LGTM")]
		public uint mLastGameTime;

		[TdfMember("LSOP")]
		public uint mLastOppo;

		[TdfMember("OPNM")]
		public string mLastOppoName;

		[TdfMember("CIMC")]
		public uint mMemberCount;

		[TdfMember("MSCO")]
		public SortedDictionary<MemberOnlineStatus, ushort> mMemberOnlineStatusCounts;

		[TdfMember("RVCN")]
		public uint mRivalCount;

	}
}
