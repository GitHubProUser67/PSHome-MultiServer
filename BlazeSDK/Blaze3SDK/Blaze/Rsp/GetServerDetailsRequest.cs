using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct GetServerDetailsRequest
	{

		[TdfMember("SID")]
		public uint mServerId;

	}
}
