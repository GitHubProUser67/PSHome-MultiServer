using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct SetConnectionStateRequest
	{

		[TdfMember("ACTV")]
		public bool mIsActive;

	}
}
