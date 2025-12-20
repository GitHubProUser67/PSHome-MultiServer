using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct SetNetworkQosRequest
	{

		[TdfMember("NQOS")]
		public Util.NetworkQosData mNetworkQosData;

	}
}
