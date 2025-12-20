using Tdf;

namespace Blaze3SDK.Blaze.Mail
{
	[TdfStruct]
	public struct SendMailToSelfRequest
	{

		[TdfMember("MTPL")]
		public string mPurpose;

		[TdfMember("MNAP")]
		public SortedDictionary<string, string> mVariableValueMap;

	}
}
