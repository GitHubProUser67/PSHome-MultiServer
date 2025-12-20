using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct GetConfigResponse
	{

		[TdfMember("ACFG")]
		public AdminConfig mAdminConfig;

		[TdfMember("GPVS")]
		public string mGameProtocolVersionString;

		[TdfMember("MSET")]
		public List<PresetSettingConfig> mMapRotationSettings;

		[TdfMember("MLST")]
		public List<MapRotation> mMapRotations;

		[TdfMember("PSLS")]
		public List<RspPingSiteInfo> mPingSites;

		[TdfMember("PSET")]
		public List<PresetSettingConfig> mPresetSettings;

		[TdfMember("PLST")]
		public List<Preset> mPresets;

	}
}
