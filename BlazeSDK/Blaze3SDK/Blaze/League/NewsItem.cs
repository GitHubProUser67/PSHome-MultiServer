using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct NewsItem
	{

		[TdfMember("TIME")]
		public uint mCreationTime;

		[TdfMember("CREA")]
		public LeagueUser mCreator;

		[TdfMember("FMT")]
		public NewsFormat mFormat;

		[TdfMember("NTYP")]
		public NewsMsgType mMsgType;

		[TdfMember("NEWS")]
		public string mNews;

		[TdfMember("NWID")]
		public uint mNewsId;

		[TdfMember("PARM")]
		public List<NewsItemParam> mParams;

	}
}
