using Tdf;

namespace Blaze3SDK.Blaze.DynamicInetFilter
{
	[TdfStruct]
	public struct CidrBlock
	{

		[TdfMember("IP")]
		public string mIp;

		[TdfMember("PLEN")]
		public uint mPrefixLength;

	}
}
