using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ReplicatedMapsVersionUpdate
	{

		[TdfMember("CCMU")]
		public bool mCachedClubDataMapIsUpdated;

		[TdfMember("CCMV")]
		public uint mCachedClubDataMapVersion;

		[TdfMember("CMIV")]
		public uint mCachedMemberInfoMapVersion;

		[TdfMember("MOSU")]
		public bool mCachedMemberOnlineStatusMapIsUpdated;

		[TdfMember("MOSV")]
		public uint mCachedMemberOnlineStatusMapVersion;

		[TdfMember("CLID")]
		public uint mLastUpdatedClubId;

	}
}
