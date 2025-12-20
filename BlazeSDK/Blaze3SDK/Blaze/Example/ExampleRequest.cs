using Tdf;

namespace Blaze3SDK.Blaze.Example
{
	[TdfStruct]
	public struct ExampleRequest
	{

		[TdfMember("NMAP")]
		public SortedDictionary<string, Nested> mNestedMap;

		[TdfMember("NUM")]
		public int mNum;

		[TdfMember("SMAP")]
		public SortedDictionary<string, string> mStringMap;

		[TdfMember("TEXT")]
		public string mText;

	}
}
