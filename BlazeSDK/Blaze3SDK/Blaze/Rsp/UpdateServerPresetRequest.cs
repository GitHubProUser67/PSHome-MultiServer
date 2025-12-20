using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateServerPresetRequest
	{

		[TdfMember("PRES")]
		public Preset mPreset;

		[TdfMember("SID")]
		public uint mServerId;

	}
}
