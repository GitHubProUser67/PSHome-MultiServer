using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct FindLeaguesAsyncNotification
	{

		[TdfMember("LEAG")]
		public League mLeague;

		[TdfMember("SQID")]
		public uint mSequenceId;

	}
}
