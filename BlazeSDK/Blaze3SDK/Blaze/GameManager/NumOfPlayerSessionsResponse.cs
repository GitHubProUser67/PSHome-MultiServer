using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct(0xC943ECC3)]
	public struct NumOfPlayerSessionsResponse
	{

		[TdfMember("NOMM")]
		public uint mNumOfPlayerSessions;

	}
}
