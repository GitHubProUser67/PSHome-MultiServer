using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct GetNewsRequest
	{

		[TdfMember("FRST")]
		public ushort mFirstItem;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("LOC")]
		public uint mLocale;

		[TdfMember("NTYP")]
		public NewsMsgType mMsgType;

		[TdfMember("NUM")]
		public ushort mNumItems;

	}
}
