using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct SetMetadataRequest
	{

		[TdfMember("SMET")]
		public byte mIsStringMetadata;

		[TdfMember("LGID")]
		public uint mLeagueId;

		[TdfMember("GMID")]
		public long mMemberId;

		[TdfMember("META")]
		public byte[] mMetadata;

		[TdfMember("CRC")]
		public uint mRosterCrc;

	}
}
