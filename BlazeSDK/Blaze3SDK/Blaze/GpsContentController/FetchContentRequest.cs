using Tdf;

namespace Blaze3SDK.Blaze.GpsContentController
{
	[TdfStruct]
	public struct FetchContentRequest
	{

		[TdfMember("COID")]
		public BlazeObjectId mContentId;

	}
}
