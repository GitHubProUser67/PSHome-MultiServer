using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct DeviceLoginGuestRequest
	{

		[TdfMember("DVID")]
		public ulong mDeviceId;

	}
}
