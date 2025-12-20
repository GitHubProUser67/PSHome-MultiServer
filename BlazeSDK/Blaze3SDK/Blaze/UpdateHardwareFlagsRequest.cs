using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UpdateHardwareFlagsRequest
	{

		[TdfMember("HWFG")]
		public HardwareFlags mHardwareFlags;

	}
}
