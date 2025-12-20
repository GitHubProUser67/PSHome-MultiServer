using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct SetPlaygroupAttributesRequest
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mPlaygroupAttributes;

		[TdfMember("PGID")]
		public uint mPlaygroupId;

	}
}
