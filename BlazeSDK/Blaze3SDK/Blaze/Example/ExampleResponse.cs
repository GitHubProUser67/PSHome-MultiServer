using Tdf;

namespace Blaze3SDK.Blaze.Example
{
	[TdfStruct]
	public struct ExampleResponse
	{

		[TdfMember("MSG")]
		public string mMessage;

		[TdfMember("LIST")]
		public List<int> mMyList;

		[TdfMember("MAP")]
		public SortedDictionary<int, ExampleRequest> mMyMap;

		[TdfMember("ENUM")]
		public ExampleResponseEnum mRegularEnum;

		public enum ExampleResponseEnum : int
		{
			EXAMPLE_ENUM_UNKNOWN = 0,
			EXAMPLE_ENUM_SUCCESS = 1,
			EXAMPLE_ENUM_FAILED = 2,
		}

	}
}
