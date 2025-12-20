using Tdf;

namespace Blaze3SDK.Blaze.Example
{
	[TdfStruct]
	public struct Nested
	{

		[TdfMember("NUM")]
		public int mNum;

		[TdfMember("NMPA")]
		public SortedDictionary<string, string> mStringMap;

		[TdfMember("TEXT")]
		public string mText;

	}
}
