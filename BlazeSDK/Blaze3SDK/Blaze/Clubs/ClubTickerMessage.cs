using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubTickerMessage
	{

		[TdfMember("TIMD")]
		public string mMetadata;

		[TdfMember("TITX")]
		public string mText;

		[TdfMember("TSTM")]
		public uint mTimestamp;

	}
}
