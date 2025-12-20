using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct CreateServerResponse
	{

		[TdfMember("SID")]
		public uint mServerId;

	}
}
