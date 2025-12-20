using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct PresetSetting
	{

		[TdfMember("KEY")]
		public string mKey;

		[TdfMember("LOCK")]
		public bool mLocked;

		[TdfMember("VAL")]
		public short mValue;

	}
}
