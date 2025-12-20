using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct Preset
	{

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("PRED")]
		public bool mPredefined;

		[TdfMember("PID")]
		public byte mPresetId;

		[TdfMember("RANK")]
		public bool mRanked;

		[TdfMember("SLST")]
		public List<PresetSetting> mSettings;

		[TdfMember("TYPE")]
		public string mType;

	}
}
