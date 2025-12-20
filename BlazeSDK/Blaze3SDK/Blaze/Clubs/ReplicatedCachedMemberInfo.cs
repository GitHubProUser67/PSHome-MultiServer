using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct(0x66196E00)]
	public struct ReplicatedCachedMemberInfo
	{

		[TdfMember("SNTD")]
		public uint mMembershipSinceTime;

		[TdfMember("MSTA")]
		public MembershipStatus mMembershipStatus;

		[TdfMember("MMDA")]
		public SortedDictionary<string, string> mMetaData;

	}
}
