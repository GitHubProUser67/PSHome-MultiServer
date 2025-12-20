using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct RestartServerRequest
	{

		[TdfMember("SID")]
		public uint mServerId;

	}
}
