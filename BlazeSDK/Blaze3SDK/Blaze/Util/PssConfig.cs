using Tdf;

namespace Blaze3SDK.Blaze.Util
{
	[TdfStruct]
	public struct PssConfig
	{

		[TdfMember("ADRS")]
		public string mAddress;

		[TdfMember("RPRT")]
		public PssReportTypes mInitialReportTypes;

		[TdfMember("CSIG")]
		public byte[] mNpCommSignature;

		[TdfMember("OIDS")]
		public List<string> mOfferIds;

		[TdfMember("PORT")]
		public uint mPort;

		[TdfMember("PJID")]
		public string mProjectId;

		[TdfMember("TIID")]
		public uint mTitleId;

	}
}
