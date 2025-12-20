using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UpdateUserSessionAttributeRequest
	{

		[TdfMember("ATID")]
		public uint mKey;

		[TdfMember("OPER")]
		public bool mRemove;

		[TdfMember("VALU")]
		public int mValue;

	}
}
