using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubSettings
    {

        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("ABRV")]
        [StringLength(30)]
        public string mAbbrev;

        [TdfMember("ARPT")]
        public ArtPackageType mArtPackageType;

        [TdfMember("BNID")]
        public uint mBannerId;

        [TdfMember("CASF")]
        public ClubArtSettingsFlags mClubArtSettingsFlags;

        [TdfMember("CLAF")]
        public ClubAcceptanceFlags mAcceptanceFlags;

        [TdfMember("CLCS")]
        public CustClubSettings mCustClubSettings;

        /// <summary>
        /// Max String Length: 65
        /// </summary>
        [TdfMember("CLDS")]
        [StringLength(65)]
        public string mDescription;

        /// <summary>
        /// Max String Length: 2048
        /// </summary>
        [TdfMember("CLMD")]
        [StringLength(2048)]
        public string mMetaData;

        [TdfMember("CLMT")]
        public MetaDataType mMetaDataType;

        [TdfMember("CLRG")]
        public uint mRegion;

        /// <summary>
        /// Max String Length: 3
        /// </summary>
        [TdfMember("LANG")]
        [StringLength(3)]
        public string mLanguage;

        [TdfMember("LOID")]
        public uint mLogoId;

        [TdfMember("LUPD")]
        public int mLastSeasonLevelUpdate;

        [TdfMember("PLVL")]
        public uint mPreviousSeasonLevel;

        [TdfMember("SLVL")]
        public uint mSeasonLevel;

        [TdfMember("TMID")]
        public uint mTeamId;

    }
}
