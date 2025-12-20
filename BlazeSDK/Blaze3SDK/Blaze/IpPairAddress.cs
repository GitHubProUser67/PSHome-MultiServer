using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct IpPairAddress
	{

		[TdfMember("EXIP")]
		public IpAddress mExternalAddress;

		[TdfMember("INIP")]
		public IpAddress mInternalAddress;

	}
}
