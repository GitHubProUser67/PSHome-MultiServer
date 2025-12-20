using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct PresetSettingConfig
	{

		[TdfMember("DEF")]
		public short mDefault;

		[TdfMember("DESC")]
		public string mDescription;

		[TdfMember("KEY")]
		public string mKey;

		[TdfMember("MAX")]
		public short mMax;

		[TdfMember("MIN")]
		public short mMin;

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("RANK")]
		public bool mRanked;

	}
}
