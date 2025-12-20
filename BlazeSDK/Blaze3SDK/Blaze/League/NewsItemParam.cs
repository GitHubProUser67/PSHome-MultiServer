using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct NewsItemParam
	{

		[TdfMember("TYPE")]
		public NewsParamType mType;

		[TdfMember("VAL")]
		public string mValue;

	}
}
