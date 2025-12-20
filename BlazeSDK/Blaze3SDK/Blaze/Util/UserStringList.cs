using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct UserStringList
	{

		[TdfMember("UTXT")]
		public List<UserText> mTextList;

	}
}
