using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct ConsoleCreateAccountResponse
	{

		[TdfMember("RSLT")]
		public CreateResult mCreateResult;

		[TdfMember("SESS")]
		public SessionInfo mSessionInfo;

		public enum CreateResult : int
		{
			CREATED = 0,
			ASSOCIATED = 1,
			ACTIVATED = 2,
		}

	}
}
