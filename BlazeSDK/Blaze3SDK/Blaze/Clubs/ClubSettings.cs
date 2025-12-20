using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubSettings
	{

		[TdfMember("CLAF")]
		public ClubAcceptanceFlags mAcceptanceFlags;

		[TdfMember("ARPT")]
		public ArtPackageType mArtPackageType;

		[TdfMember("BNID")]
		public uint mBannerId;

		[TdfMember("CASF")]
		public ClubArtSettingsFlags mClubArtSettingsFlags;

		[TdfMember("CLCS")]
		public CustClubSettings mCustClubSettings;

		[TdfMember("CLDS")]
		public string mDescription;

		[TdfMember("HSPW")]
		public bool mHasPassword;

		[TdfMember("LANG")]
		public string mLanguage;

		[TdfMember("LUPD")]
		public int mLastSeasonLevelUpdate;

		[TdfMember("LOID")]
		public uint mLogoId;

		[TdfMember("CLMD")]
		public string mMetaData;

		[TdfMember("CLD2")]
		public string mMetaData2;

		[TdfMember("CLMT")]
		public MetaDataType mMetaDataType;

		[TdfMember("CLT2")]
		public MetaDataType mMetaDataType2;

		[TdfMember("NUQN")]
		public string mNonUniqueName;

		[TdfMember("PSWD")]
		public string mPassword;

		[TdfMember("PLVL")]
		public uint mPreviousSeasonLevel;

		[TdfMember("CLRG")]
		public uint mRegion;

		[TdfMember("SLVL")]
		public uint mSeasonLevel;

		[TdfMember("TMID")]
		public uint mTeamId;

	}
}
