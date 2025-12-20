using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct ResetPlaygroupSessionResponse
	{

		[TdfMember("PGID")]
		public uint mId;

		[TdfMember("PRES")]
		public bool mSessionChanging;

	}
}
