using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct FindClubsRequest
    {

        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("ABRV")]
        [StringLength(30)]
        public string mAbbrev;

        [TdfMember("ACEF")]
        public ClubAcceptanceFlags mAcceptanceFlags;

        [TdfMember("ACMS")]
        public ClubAcceptanceFlags mAcceptanceMask;

        [TdfMember("ATID")]
        public bool mAnyTeamId;

        [TdfMember("CFLI")]
        public List<uint> mClubFilterList;

        [TdfMember("CMMO")]
        public List<uint> mMinMemberOnlineStatusCounts;

        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("CNAM")]
        [StringLength(30)]
        public string mName;

        [TdfMember("CREG")]
        public uint mRegion;

        /// <summary>
        /// Max String Length: 3
        /// </summary>
        [TdfMember("LANG")]
        [StringLength(3)]
        public string mLanguage;

        [TdfMember("MAMC")]
        public uint mMaxMemberCount;

        [TdfMember("MIMC")]
        public uint mMinMemberCount;

        [TdfMember("MXRC")]
        public uint mMaxResultCount;

        [TdfMember("OFRC")]
        public uint mOffset;

        [TdfMember("SEAL")]
        public uint mSeasonLevel;

        [TdfMember("SKMD")]
        public byte mSkipMetadata;

        [TdfMember("TMID")]
        public uint mTeamId;

        [TdfMember("UFLI")]
        public List<uint> mMemberFilterList;

    }
}
