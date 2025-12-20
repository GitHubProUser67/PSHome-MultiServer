using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct FilteredUserText
	{

		[TdfMember("UTXT")]
		public string mFilteredText;

		[TdfMember("DIRT")]
		public FilterResult mResult;

	}
}
