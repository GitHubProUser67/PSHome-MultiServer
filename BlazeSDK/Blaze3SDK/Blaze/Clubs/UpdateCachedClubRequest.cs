using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct UpdateCachedClubRequest
	{

		[TdfMember("BLID")]
		public long mBlazeId;

		[TdfMember("MCIS")]
		public SortedDictionary<uint, ReplicatedCachedClubData> mMapClubIdToCachedClubData;

		[TdfMember("MCMI")]
		public SortedDictionary<uint, ReplicatedCachedMemberInfo> mMapClubIdToMemberInfo;

		[TdfMember("UPRS")]
		public UpdateReason mReason;

	}
}
