using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UserStatus
	{

		[TdfMember("ID")]
		public long mBlazeId;

		[TdfMember("FLGS")]
		public UserDataFlags mStatusFlags;

	}
}
