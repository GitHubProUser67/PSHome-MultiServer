using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct UpdateCachedClubForUsersRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("MBMI")]
		public SortedDictionary<long, ReplicatedCachedMemberInfo> mMapBlazeIdToMemberInfo;

		[TdfMember("UPRS")]
		public UpdateReason mReason;

	}
}
