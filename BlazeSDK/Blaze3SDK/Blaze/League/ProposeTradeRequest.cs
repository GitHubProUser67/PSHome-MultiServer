using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct ProposeTradeRequest
	{

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("FORP")]
		public uint mOriginatorPlayerId;

		[TdfMember("LATT")]
		public long mRecipientId;

		[TdfMember("LATP")]
		public uint mRecipientPlayerId;

	}
}
