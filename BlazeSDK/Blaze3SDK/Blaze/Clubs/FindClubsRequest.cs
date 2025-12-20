using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct FindClubsRequest
	{

		[TdfMember("ACEF")]
		public ClubAcceptanceFlags mAcceptanceFlags;

		[TdfMember("ACMS")]
		public ClubAcceptanceFlags mAcceptanceMask;

		[TdfMember("ADID")]
		public bool mAnyDomain;

		[TdfMember("ATID")]
		public bool mAnyTeamId;

		[TdfMember("DMID")]
		public uint mClubDomainId;

		[TdfMember("CFLI")]
		public List<uint> mClubFilterList;

		[TdfMember("CODR")]
		public ClubsOrder mClubsOrder;

		[TdfMember("CLTI")]
		public bool mIncludeClubTags;

		[TdfMember("LANG")]
		public string mLanguage;

		[TdfMember("LGTM")]
		public uint mLastGameTimeOffset;

		[TdfMember("MAMC")]
		public uint mMaxMemberCount;

		[TdfMember("MXRC")]
		public uint mMaxResultCount;

		[TdfMember("UFLI")]
		public List<long> mMemberFilterList;

		[TdfMember("MIMC")]
		public uint mMinMemberCount;

		[TdfMember("MMSC")]
		public SortedDictionary<MemberOnlineStatus, ushort> mMinMemberOnlineStatusCounts;

		[TdfMember("CNAM")]
		public string mName;

		[TdfMember("NUQN")]
		public string mNonUniqueName;

		[TdfMember("OFRC")]
		public uint mOffset;

		[TdfMember("ODMD")]
		public OrderMode mOrderMode;

		[TdfMember("PSWD")]
		public PasswordOption mPasswordOption;

		[TdfMember("CREG")]
		public uint mRegion;

		[TdfMember("SEAL")]
		public uint mSeasonLevel;

		[TdfMember("SKCT")]
		public bool mSkipCalcDbRows;

		[TdfMember("SKMD")]
		public byte mSkipMetadata;

		[TdfMember("CLTG")]
		public List<string> mTagList;

		[TdfMember("CLTO")]
		public TagSearchOperation mTagSearchOperation;

		[TdfMember("TMID")]
		public uint mTeamId;

	}
}
