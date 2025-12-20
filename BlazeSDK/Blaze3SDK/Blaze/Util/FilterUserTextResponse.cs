using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct FilterUserTextResponse
	{

		[TdfMember("TLST")]
		public List<FilteredUserText> mFilteredTextList;

	}
}
