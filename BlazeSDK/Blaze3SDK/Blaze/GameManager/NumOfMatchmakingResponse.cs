using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct(0x95EBFC4D)]
	public struct NumOfMatchmakingResponse
	{

		[TdfMember("NOMM")]
		public uint mNumOfMatchmakingSessions;

	}
}
