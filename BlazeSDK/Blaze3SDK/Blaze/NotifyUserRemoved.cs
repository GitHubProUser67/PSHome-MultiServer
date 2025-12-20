using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct NotifyUserRemoved
	{

		[TdfMember("BUID")]
		public long mUserId;

	}
}
