using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct Trade
	{

		[TdfMember("CRTM")]
		public uint mCreationTime;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("FORM")]
		public LeagueUser mOriginator;

		[TdfMember("FORP")]
		public uint mOriginatorPlayerId;

		[TdfMember("LATT")]
		public LeagueUser mRecipient;

		[TdfMember("LATP")]
		public uint mRecipientPlayerId;

		[TdfMember("TDID")]
		public uint mTradeId;

	}
}
