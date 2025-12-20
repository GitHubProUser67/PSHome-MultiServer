using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ListDeviceAccountsRequest
	{

		[TdfMember("DVID")]
		public ulong mDeviceId;

	}
}
