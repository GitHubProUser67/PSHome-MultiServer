using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct PostNewsRequest
	{

		[TdfMember("FRMT")]
		public NewsFormat mFormat;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("NTYP")]
		public NewsMsgType mMsgType;

		[TdfMember("NEWS")]
		public string mNews;

	}
}
