using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct AwardSettings
	{

		[TdfMember("AWCS")]
		public uint mAwardChecksum;

		[TdfMember("AWID")]
		public uint mAwardId;

		[TdfMember("AWNA")]
		public string mAwardName;

		[TdfMember("AWUR")]
		public string mAwardURL;

	}
}
