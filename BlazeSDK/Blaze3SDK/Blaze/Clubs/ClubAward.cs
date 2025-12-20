using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ClubAward
	{

		[TdfMember("AWID")]
		public uint mAwardId;

		[TdfMember("IMCS")]
		public int mAwardImgCheckSum;

		[TdfMember("AWIU")]
		public string mAwardImgURL;

		[TdfMember("CAWI")]
		public uint mCount;

		[TdfMember("LUDT")]
		public uint mLastUpdateTime;

	}
}
