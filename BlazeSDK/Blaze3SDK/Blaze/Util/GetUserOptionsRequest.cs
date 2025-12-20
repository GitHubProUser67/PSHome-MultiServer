using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct GetUserOptionsRequest
	{

		[TdfMember("UID")]
		public long mUserId;

	}
}
