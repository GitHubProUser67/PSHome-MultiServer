using Tdf;

namespace Blaze3SDK.Blaze.GpsContentController
{
	[TdfStruct]
	public struct FilePetitionRequest
	{

		[TdfMember("ANVP")]
		public SortedDictionary<string, string> mAttributeMap;

		[TdfMember("COTY")]
		public string mComplaintType;

		[TdfMember("COID")]
		public BlazeObjectId mContentId;

		[TdfMember("PTDE")]
		public string mPetitionDetail;

		[TdfMember("SUBJ")]
		public string mSubject;

		[TdfMember("TRGT")]
		public List<long> mTargetUsers;

		[TdfMember("TMZO")]
		public string mTimeZone;

	}
}
