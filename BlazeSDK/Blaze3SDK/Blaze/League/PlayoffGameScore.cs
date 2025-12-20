using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct PlayoffGameScore
	{

		[TdfMember("SIMG")]
		public byte mIsSimulatedGame;

		[TdfMember("P1SC")]
		public int mPlayer1Score;

		[TdfMember("P2SC")]
		public int mPlayer2Score;

	}
}
