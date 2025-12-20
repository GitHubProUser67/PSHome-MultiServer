using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct UserSessionDisconnectReason
	{

		[TdfMember("SDR")]
		public DisconnectReason mDisconnectReason;

		public enum DisconnectReason : int
		{
			DUPLICATE_LOGIN = 0,
		}

	}
}
