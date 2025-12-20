using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GetUserSetGameListSubscriptionRequest
	{

		[TdfMember("USID")]
		public BlazeObjectId mUserSetId;

	}
}
