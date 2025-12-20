using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GameAttributeCensusData
	{

		[TdfMember("ATTN")]
		public string mAttributeName;

		[TdfMember("ATTV")]
		public string mAttributevalue;

		[TdfMember("NOFG")]
		public uint mNumOfGames;

		[TdfMember("NOFP")]
		public uint mNumOfPlayers;

	}
}
