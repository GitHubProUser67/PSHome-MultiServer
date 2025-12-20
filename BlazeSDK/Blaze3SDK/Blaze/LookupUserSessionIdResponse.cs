using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct LookupUserSessionIdResponse
	{

		[TdfMember("SID")]
		public List<uint> mUsersessionidList;

	}
}
