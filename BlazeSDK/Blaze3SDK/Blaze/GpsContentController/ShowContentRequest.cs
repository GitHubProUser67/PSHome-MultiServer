using Tdf;

namespace Blaze3SDK.Blaze.GpsContentController
{
	[TdfStruct]
	public struct ShowContentRequest
	{

		[TdfMember("COID")]
		public BlazeObjectId mContentId;

		[TdfMember("SHOW")]
		public bool mShow;

	}
}
