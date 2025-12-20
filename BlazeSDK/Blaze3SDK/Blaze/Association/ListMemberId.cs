using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct ListMemberId
	{

		[TdfMember("BLID")]
		public long mBlazeId;

		[TdfMember("XREF")]
		public ulong mExternalId;

		[TdfMember("XTYP")]
		public ExternalSystemId mExternalSystemId;

		[TdfMember("PNAM")]
		public string mPersonaName;

	}
}
