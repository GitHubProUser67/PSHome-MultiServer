using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct ResetPlaygroupSessionRequest
	{

		[TdfMember("PGID")]
		public uint mId;

		[TdfMember("PRES")]
		public bool mUsesPresence;

	}
}
