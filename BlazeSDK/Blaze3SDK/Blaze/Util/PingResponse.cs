using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct PingResponse
	{

		[TdfMember("STIM")]
		public uint mServerTime;

	}
}
