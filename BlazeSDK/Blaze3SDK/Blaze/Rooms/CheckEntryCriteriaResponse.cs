using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct CheckEntryCriteriaResponse
	{

		[TdfMember("FCRI")]
		public string mFailedCriteria;

		[TdfMember("PASS")]
		public bool mPassed;

	}
}
