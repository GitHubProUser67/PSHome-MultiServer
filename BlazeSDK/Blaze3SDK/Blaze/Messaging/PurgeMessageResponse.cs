using Tdf;

namespace Blaze3SDK.Blaze.Messaging
{
	[TdfStruct]
	public struct PurgeMessageResponse
	{

		[TdfMember("MCNT")]
		public uint mCount;

	}
}
