using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct DecrementUseCount
	{

		[TdfMember("UCTC")]
		public uint mUseCountConsumed;

		[TdfMember("UCTR")]
		public uint mUseCountRemain;

	}
}
