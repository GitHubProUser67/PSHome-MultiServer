using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct NetworkQosData
	{

		[TdfMember("DBPS")]
		public uint mDownstreamBitsPerSecond;

		[TdfMember("NATT")]
		public NatType mNatType;

		[TdfMember("UBPS")]
		public uint mUpstreamBitsPerSecond;

	}
}
