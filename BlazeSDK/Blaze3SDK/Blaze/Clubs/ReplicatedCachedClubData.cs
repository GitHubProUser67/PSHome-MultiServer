using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct(0x85B5AB18)]
	public struct ReplicatedCachedClubData
	{

		[TdfMember("DMID")]
		public uint mClubDomainId;

		[TdfMember("CLST")]
		public ClubSettings mClubSettings;

		[TdfMember("NAME")]
		public string mName;

	}
}
