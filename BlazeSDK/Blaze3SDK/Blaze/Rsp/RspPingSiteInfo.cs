using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct RspPingSiteInfo
	{

		[TdfMember("ALIA")]
		public string mAlias;

		[TdfMember("CAP")]
		public uint mCapacity;

		[TdfMember("NAME")]
		public string mName;

	}
}
