using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct SetMemberAttributesRequest
	{

		[TdfMember("EID")]
		public long mBlazeId;

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mMemberAttributes;

		[TdfMember("PGID")]
		public uint mPlaygroupId;

	}
}
