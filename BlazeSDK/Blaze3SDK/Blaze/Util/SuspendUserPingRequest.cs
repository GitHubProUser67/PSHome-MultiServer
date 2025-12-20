using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct SuspendUserPingRequest
	{

		[TdfMember("TVAL")]
		public TimeValue mSuspendTime;

	}
}
