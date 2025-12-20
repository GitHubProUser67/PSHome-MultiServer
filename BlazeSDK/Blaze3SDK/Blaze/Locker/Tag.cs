using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct Tag
	{

		[TdfMember("TAG")]
		public string mTag;

		[TdfMember("TGID")]
		public int mTagId;

	}
}
