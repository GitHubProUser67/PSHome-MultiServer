using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct UpdateMemberMetadataRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("META")]
		public SortedDictionary<string, string> mMetaData;

	}
}
