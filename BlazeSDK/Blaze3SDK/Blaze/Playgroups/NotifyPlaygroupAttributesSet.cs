using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct NotifyPlaygroupAttributesSet
	{

		[TdfMember("ATTR")]
		public SortedDictionary<string, string> mPlaygroupAttributes;

		[TdfMember("PGID")]
		public uint mPlaygroupId;

	}
}
