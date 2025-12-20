using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct ClientMetrics
	{

		[TdfMember("UBFL")]
		public BlazeUpnpFlags mBlazeFlags;

		[TdfMember("UDEV")]
		public string mDeviceInfo;

		[TdfMember("UFLG")]
		public ushort mFlags;

		[TdfMember("ULRC")]
		public int mLastRsltCode;

		[TdfMember("UNAT")]
		public ushort mNatType;

		[TdfMember("USTA")]
		public UpnpStatus mStatus;

		[TdfMember("UWAN")]
		public uint mWanIpAddr;

	}
}
