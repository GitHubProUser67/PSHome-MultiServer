using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	[TdfStruct]
	public struct GetStatDescsRequest
	{

		[TdfMember("CAT")]
		public string mCategory;

		[TdfMember("STAT")]
		public List<string> mStatNames;

	}
}
