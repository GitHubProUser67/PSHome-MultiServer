using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct GetMyTournamentIdResponse
	{

		[TdfMember("TNID")]
		public uint mId;

		[TdfMember("ACTI")]
		public bool mIsActive;

	}
}
