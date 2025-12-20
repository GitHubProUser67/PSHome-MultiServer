using Tdf;

namespace Blaze3SDK.Blaze.Locker
{
	[TdfStruct]
	public struct LeaderboardFilter
	{

		[TdfMember("NAME")]
		public string mName;

		[TdfMember("OP")]
		public string mOperator;

		[TdfMember("VALU")]
		public string mValue;

	}
}
