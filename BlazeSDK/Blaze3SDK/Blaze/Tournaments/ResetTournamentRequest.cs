using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct ResetTournamentRequest
	{

		[TdfMember("BZID")]
		public long mBlazeId;

		[TdfMember("TNID")]
		public uint mTournamentId;

	}
}
