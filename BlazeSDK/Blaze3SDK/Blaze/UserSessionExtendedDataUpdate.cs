using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UserSessionExtendedDataUpdate
	{

		[TdfMember("DATA")]
		public UserSessionExtendedData mExtendedData;

		[TdfMember("USID")]
		public long mUserId;

	}
}
