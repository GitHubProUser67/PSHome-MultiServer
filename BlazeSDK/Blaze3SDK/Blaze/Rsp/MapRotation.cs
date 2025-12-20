using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct MapRotation
	{

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("MRID")]
		public byte mMapRotationId;

		[TdfMember("MLST")]
		public List<MapRotationEntry> mMaps;

		[TdfMember("MOD")]
		public string mMod;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("RAND")]
		public bool mRandomStartMap;

		[TdfMember("SLST")]
		public List<PresetSetting> mSettings;

	}
}
